CREATE TABLE [dbo].[AttendanceUploadingResult] (
    [SessionId]  NVARCHAR (100) NOT NULL,
    [RowNumber]  INT            NOT NULL,
    [ResultType] SMALLINT       NOT NULL,
    [Message]    NVARCHAR (MAX) NULL
);

