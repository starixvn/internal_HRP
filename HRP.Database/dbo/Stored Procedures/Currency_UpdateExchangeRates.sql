

CREATE PROCEDURE [dbo].[Currency_UpdateExchangeRates] @ExchangeRateMonth DATE
	,@ExchangeRates VARCHAR(MAX)
	,@UpdatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20171226@Elton.Ton
Task: update currency exchange rates
DB: bodb_HRP
Original:
*/
BEGIN
	SET NOCOUNT ON

	-- For write log
	DECLARE @startRun DATETIME
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@logDataOld NVARCHAR(MAX)
		,@xml XML
		,@idoc INT

	SET @startRun = GETDATE()
	SET @xml = (
			SELECT c.ExchangeRateMonth AS '@erm'
				,c.FromCurrencyId AS '@fci'
				,c.ToCurrencyId AS '@tci'
				,c.ExchangeRate AS '@er'
			FROM dbo.CurrencyExchangeRate AS c WITH (NOLOCK)
			WHERE MONTH(c.ExchangeRateMonth) = MONTH(@ExchangeRateMonth)
				AND YEAR(c.ExchangeRateMonth) = YEAR(@ExchangeRateMonth)
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	CREATE TABLE #ExchangeRate (
		FromCurrencyId INT
		,ToCurrencyId INT
		,ExchangeRate DECIMAL(10, 5)
		)

	EXEC sp_xml_preparedocument @idoc OUTPUT, @ExchangeRates

	INSERT INTO #ExchangeRate (
		FromCurrencyId
		,ToCurrencyId
		,ExchangeRate
		)
	SELECT FromCurrencyId
		,ToCurrencyId
		,ExchangeRate
	FROM OPENXML(@idoc, '/Root/Row', 1) WITH (FromCurrencyId INT, ToCurrencyId INT, ExchangeRate DECIMAL(10, 5)) AS data

	IF EXISTS (SELECT 1 FROM dbo.CurrencyExchangeRate cur WHERE MONTH(cur.ExchangeRateMonth) = MONTH(@ExchangeRateMonth)
		AND YEAR(cur.ExchangeRateMonth) = YEAR(@ExchangeRateMonth))
	BEGIN
		-- Update
		UPDATE cur
		SET cur.ExchangeRate = er.ExchangeRate
		FROM dbo.CurrencyExchangeRate AS cur WITH (ROWLOCK, UPDLOCK)
			INNER JOIN #ExchangeRate AS er WITH (NOLOCK) ON MONTH(cur.ExchangeRateMonth) = MONTH(@ExchangeRateMonth)
			AND YEAR(cur.ExchangeRateMonth) = YEAR(@ExchangeRateMonth)
			AND cur.FromCurrencyId = er.FromCurrencyId
			AND cur.ToCurrencyId = er.ToCurrencyId
	END
	ELSE
	BEGIN
		--Insert new exchange rates
		INSERT INTO dbo.CurrencyExchangeRate (
			ExchangeRateMonth
			,FromCurrencyId
			,ToCurrencyId
			,ExchangeRate
			)
		SELECT @ExchangeRateMonth
			,er.FromCurrencyId
			,er.ToCurrencyId
			,er.ExchangeRate
		FROM #ExchangeRate AS er WITH (NOLOCK)
	END
		
	-- write log after update
	SET @xml = (
			SELECT c.ExchangeRateMonth AS '@erm'
				,c.FromCurrencyId AS '@fci'
				,c.ToCurrencyId AS '@tci'
				,c.ExchangeRate AS '@er'
			FROM dbo.CurrencyExchangeRate AS c WITH (NOLOCK)
			WHERE MONTH(c.ExchangeRateMonth) = MONTH(@ExchangeRateMonth)
				AND YEAR(c.ExchangeRateMonth) = YEAR(@ExchangeRateMonth)
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @UserId = @UpdatedBy
		,@SPName = 'Currency_UpdateExchangeRates'
		,@ObjectId = @UpdatedBy
		,@ObjectTypeId = 2
		,@Data = @logData
		,@OldData = @logDataOld
		,@Duration = @Duration

	DROP TABLE #ExchangeRate
END