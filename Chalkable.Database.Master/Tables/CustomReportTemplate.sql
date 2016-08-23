Create Table CustomReportTemplate
(
	Id Uniqueidentifier not null Primary Key,
	Name nvarchar(256) not null, 
	Layout nvarchar(max) not null,
	Style nvarchar(max) null 
)
