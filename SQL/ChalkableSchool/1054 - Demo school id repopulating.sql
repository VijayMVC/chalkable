create procedure spRepopulatePersonId 
	@prefix nvarchar(256)
as
begin transaction

create table #newIds
(
	Old uniqueidentifier,
	New uniqueidentifier
)

insert into #newIds
select Id, newid() from Person


declare @fkTable nvarchar(1024), @fkColumn nvarchar(1024), @constraintName nvarchar(1024)
declare @sql nvarchar (2048)

declare @References table 
(
	FkTable nvarchar(1024),
	FkColumn nvarchar(1024),
	ConstraintName nvarchar(1024)
)

insert into @References
SELECT
	K_Table = FK.TABLE_NAME,
	FK_Column = CU.COLUMN_NAME,
	Constraint_Name = C.CONSTRAINT_NAME
FROM 
	INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS C
	INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK ON C.CONSTRAINT_NAME = FK.CONSTRAINT_NAME
	INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS PK ON C.UNIQUE_CONSTRAINT_NAME = PK.CONSTRAINT_NAME
	INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU ON C.CONSTRAINT_NAME = CU.CONSTRAINT_NAME
	INNER JOIN (
	SELECT i1.TABLE_NAME, i2.COLUMN_NAME
	FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS i1
	INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE i2 ON i1.CONSTRAINT_NAME = i2.CONSTRAINT_NAME
	WHERE i1.CONSTRAINT_TYPE = 'PRIMARY KEY'
	) PT ON PT.TABLE_NAME = PK.TABLE_NAME
where PK.TABLE_NAME = 'Person' and PT.COLUMN_NAME = 'Id'



Declare PersonRelatedCursor cursor for select * from @References


open PersonRelatedCursor
fetch next from PersonRelatedCursor
into @fkTable, @fkColumn, @constraintName


while @@FETCH_STATUS = 0
begin
	set @sql = concat('alter table ', @fkTable,  ' drop ', @constraintName)
	print(@sql)
	exec(@sql)		

	set @sql = concat('update ', @fkTable, ' set ', @fkColumn, '= (select New from #newIds where Old = ',@fkColumn , ')')
	print(@sql)
	exec(@sql)		

	fetch next from PersonRelatedCursor
	into @fkTable, @fkColumn, @constraintName
end
close PersonRelatedCursor

update Person set Id = (select New from #newIds where Old = Id)
update Person set Email = concat(@prefix, email)

open PersonRelatedCursor
fetch next from PersonRelatedCursor
into @fkTable, @fkColumn, @constraintName


while @@FETCH_STATUS = 0
begin
	set @sql = concat('alter table ', @fkTable,  ' add constraint ', @constraintName, ' Foreign Key (', @fkColumn, ') References Person(Id)')
	print(@sql)
	exec(@sql)
	fetch next from PersonRelatedCursor
	into @fkTable, @fkColumn, @constraintName
end
close PersonRelatedCursor
deallocate PersonRelatedCursor




commit