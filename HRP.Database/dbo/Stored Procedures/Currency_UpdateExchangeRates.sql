/*
Created: 20171226@Elton.Ton
Task: update currency exchange rates
DB: bodb_HRP
Original:
*/
CREATE PROCEDURE [dbo].[Currency_UpdateExchangeRates] 
	@ExchangeRateMonth DATE
	,@ExchangeRates VARCHAR(MAX)
	,@UpdatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS

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
			SELECT	c.ExchangeRateMonth AS '@erm'
					,c.FromCurrency AS '@fc'
					,c.ToCurrency AS '@tc'
					,c.ExchangeRate AS '@er'
			FROM dbo.CurrencyExchangeRate AS c WITH (NOLOCK)
			WHERE MONTH(c.ExchangeRateMonth) = MONTH(@ExchangeRateMonth) AND YEAR(c.ExchangeRateMonth) = YEAR(@ExchangeRateMonth)
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	CREATE TABLE #ExchangeRate (
		FromCurrency VARCHAR(3)
		,ToCurrency VARCHAR(3)	
		,ExchangeRate DECIMAL(10,5)
	)

	EXEC sp_xml_preparedocument @idoc OUTPUT, @ExchangeRates

	INSERT INTO #ExchangeRate (
		FromCurrency
		,ToCurrency
		,ExchangeRate
	)
	SELECT	FromCurrency
			,ToCurrency
			,ExchangeRate
	FROM OPENXML(@idoc, '/Root/Row', 1) WITH (FromCurrency VARCHAR(3), ToCurrency VARCHAR(3), ExchangeRate DECIMAL(10,5)) AS data


	-- Delete not set exchange rate
	DELETE cur
	FROM dbo.CurrencyExchangeRate AS cur
		INNER JOIN #ExchangeRate AS er WITH (NOLOCK) ON 
		MONTH(cur.ExchangeRateMonth) = MONTH(@ExchangeRateMonth) 
		AND YEAR(cur.ExchangeRateMonth) = YEAR(@ExchangeRateMonth)
		AND cur.FromCurrency = er.FromCurrency AND cur.ToCurrency = er.ToCurrency
	WHERE er.ExchangeRate = 0

	-- Update
	UPDATE cur
	SET cur.ExchangeRate = er.ExchangeRate
	FROM dbo.CurrencyExchangeRate AS cur
		INNER JOIN #ExchangeRate AS er WITH (NOLOCK) ON 
		MONTH(cur.ExchangeRateMonth) = MONTH(@ExchangeRateMonth) 
		AND YEAR(cur.ExchangeRateMonth) = YEAR(@ExchangeRateMonth)
		AND cur.FromCurrency = er.FromCurrency AND cur.ToCurrency = er.ToCurrency
	WHERE er.ExchangeRate > 0 AND cur.ExchangeRate <> er.ExchangeRate

	--Insert new exchange rates
	INSERT INTO dbo.CurrencyExchangeRate(
		ExchangeRateMonth
		,FromCurrency
		,ToCurrency
		,ExchangeRate
	)
	SELECT	@ExchangeRateMonth
			,er.FromCurrency
			,er.ToCurrency
			,er.ExchangeRate
	FROM #ExchangeRate AS er WITH (NOLOCK)
	WHERE er.ExchangeRate > 0
	AND NOT EXISTS (
		SELECT 1
		FROM dbo.CurrencyExchangeRate AS cur WITH (NOLOCK)
		WHERE MONTH(cur.ExchangeRateMonth) = MONTH(@ExchangeRateMonth) 
		AND YEAR(cur.ExchangeRateMonth) = YEAR(@ExchangeRateMonth)
		AND cur.FromCurrency = er.FromCurrency AND cur.ToCurrency = er.ToCurrency
	)

	-- write log after update
	SET @xml = (
			SELECT	c.ExchangeRateMonth AS '@erm'
					,c.FromCurrency AS '@fc'
					,c.ToCurrency AS '@tc'
					,c.ExchangeRate AS '@er'
			FROM dbo.CurrencyExchangeRate AS c WITH (NOLOCK)
			WHERE MONTH(c.ExchangeRateMonth) = MONTH(@ExchangeRateMonth) AND YEAR(c.ExchangeRateMonth) = YEAR(@ExchangeRateMonth)
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