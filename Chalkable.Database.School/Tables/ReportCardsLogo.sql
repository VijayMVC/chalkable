Create Table ReportCardsLogo
(
	Id Int Not Null Primary Key identity(1,1),
	SchoolRef Int Null Constraint FK_ReportCardsLogo_School Foreign Key References School(Id),
	LogoAddress nvarchar(256)
)