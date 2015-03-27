declare @script nvarchar(max) = ''

set @script = (

SELECT ' alter table [' + o.Name + '] ' 
				 + ' alter column [' + c.Name + '] ' + (case 
										   when y.name = 'varchar' or y.name = 'nvarchar' then ' nvarchar(' +  (case when c.max_length = -1 or c.max_length > 4000  then 'max' else cast(c.max_length  as nvarchar(max)) end) + ')' 
										   when y.name = 'char' or y.name = 'nchar' then ' nchar(' +  (case when c.max_length = -1 then 'max' else cast(c.max_length  as nvarchar(max)) end) + ')' 
										   else '' end)
				 + (case when c.is_nullable = 1 then ' null' else ' not null ' end)
				 + ' '
		 
FROM     sys.columns c 
         JOIN sys.objects o ON o.object_id = c.object_id 
		 JOIN sys.types y ON y.system_type_id = c.system_type_id
where (y.name = 'varchar' or y.name = 'char') and o.type = 'U' 
	and not exists (
				 select * from information_schema.table_constraints tc
				 left JOIN information_schema.key_column_usage kcu on kcu.Constraint_Name = tc.Constraint_Name 
				 where c.Name = kcu.Column_name and tc.constraint_type = 'UNIQUE' and tc.table_name = o.name
			   )
ORDER BY o.Name, c.column_id
FOR XML PATH(''))


set @script = @script + (SELECT 
		' alter table [' + o.Name + '] ' + ' drop constraint [' + tc.Constraint_Name + '] ' +
		' alter table [' + o.Name + '] ' 
				 + ' alter column [' + c.Name + '] ' + (case 
										   when y.name = 'varchar' or y.name = 'nvarchar' then ' nvarchar(' +  (case when c.max_length = -1 or c.max_length > 4000  then 'max' else cast(c.max_length  as nvarchar(max)) end) + ')' 
										   when y.name = 'char' or y.name = 'nchar' then ' nchar(' +  (case when c.max_length = -1 then 'max' else cast(c.max_length  as nvarchar(max)) end) + ')' 
										   else '' end)
				 + (case when c.is_nullable = 1 then ' null' else ' not null ' end)
				 + ' ' +
		' alter table [' + o.Name + '] ' + ' add constraint [' + tc.Constraint_Name + '] unique (' 
		+ (
			select kcu.column_name + (case when COUNT(*) OVER (PARTITION BY kcu.table_name)  > kcu.ordinal_position then ',' else '' end)
		    from information_schema.key_column_usage kcu
			where kcu.Constraint_Name = tc.Constraint_Name 
			order by kcu.ordinal_position
			for xml path('')
		   )
		+ ')'
		 
FROM     sys.columns c 
         JOIN sys.objects o ON o.object_id = c.object_id 
		 JOIN sys.types y ON y.system_type_id = c.system_type_id
		 JOIN information_schema.table_constraints tc on tc.Table_name = o.Name
where (y.name = 'varchar' or y.name = 'char') and o.type = 'U' and tc.constraint_type = 'UNIQUE'
	and exists (
				  select * from information_schema.key_column_usage kcu
				  where c.Name = kcu.Column_name and kcu.Constraint_Name = tc.Constraint_Name 
			   )
ORDER BY o.Name, c.column_id
FOR XML PATH(''))



set @script = @script + (SELECT 'drop type [dbo].[' + substring(t.name, 4, len(t.name) - 12) + '] ' +
	   'create type [' +  substring(t.name, 4, len(t.name) - 12) + '] as table ( '+
			(select '[' + c.name + ']' + (case 
										   when y.name = 'varchar' or y.name = 'nvarchar' then ' nvarchar(' +  (case when c.max_length = -1 or c.max_length > 4000 then 'max' else cast(c.max_length  as nvarchar(max)) end) + ')' 
										   when y.name = 'char' or y.name = 'nchar' then ' nchar(' +  (case when c.max_length = -1 then 'max' else cast(c.max_length  as nvarchar(max)) end) + ')' 
										   when y.name = 'decimal' then ' [decimal](' + cast(c.[precision] as nvarchar) + ',' + cast(c.scale as nvarchar) + ')' 
										   else ' [' +  y.name + ']' end) 
				  + ' ' + (case when c.is_nullable = 1 then ' null ' else ' not null ' end)
				  + (case when c.column_id = (select count(*) from sys.columns c  where c.object_id = t.object_id) then '' else ', ' end)
		  from sys.columns c 
		  JOIN sys.types y ON y.system_type_id = c.system_type_id
		  where c.object_id = t.object_id and y.name <> 'sysname'
		  order by c.column_id
		  FOR XML PATH ('')) +
	   ') ' 
FROM  sys.objects t   
where t.type = 'TT' and t.object_id in (select c.object_id from  sys.columns c 
										JOIN sys.types y ON y.system_type_id = c.system_type_id
										where (y.name = 'varchar' or y.name = 'char') and lower(c.name) <> lower('code'))
for xml path(''))


select @script
execute(@script)


 