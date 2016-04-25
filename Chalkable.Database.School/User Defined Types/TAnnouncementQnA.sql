CREATE TYPE [dbo].[TAnnouncementQnA] AS TABLE (
    [id]              INT            NOT NULL,
    [AnnouncementRef] INT            NOT NULL,
    [AskerRef]        INT            NOT NULL,
    [Question]        NVARCHAR (MAX) NOT NULL,
    [Answer]          NVARCHAR (MAX) NULL,
    [State]           INT            NOT NULL,
    [AnsweredTime]    DATETIME2 (7)  NULL,
    [QuestionTime]    DATETIME2 (7)  NULL,
    [AnswererRef]     INT            NULL);

