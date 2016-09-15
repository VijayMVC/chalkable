Create Table CustomReportTemplate
(
	Id Uniqueidentifier not null Primary Key,
	Name nvarchar(256) not null, 
	Layout nvarchar(max) not null,
	Style nvarchar(max) null,
	HeaderRef Uniqueidentifier null constraint FK_CustomReportTemplate_Header foreign key references CustomReportTemplate(Id),
	FooterRef Uniqueidentifier null constraint FK_CustomReportTemplate_Footer foreign key references CustomReportTemplate(Id),
	[Type] int not null
)
