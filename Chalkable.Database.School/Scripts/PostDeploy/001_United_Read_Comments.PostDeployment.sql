--001_Drop_GalleryCategoryRef_From_LessonPlan.PostDeploy.sql

If (Object_Id('[dbo].[LessonPlan]') Is Not Null And Exists(Select * From sys.columns Where Name = '[GalleryCategoryRef]'))
Begin
	ALTER TABLE [dbo].[LessonPlan]
	DROP COLUMN [GalleryCategoryRef]
End

--002_Replace_SchoolYear_to_AcadYear_in_PersonSetting.PostDeployment2.sql

IF TYPE_ID(N'dbo.THierarchy') IS NOT NULL
	DROP TYPE dbo.THierarchy
GO

CREATE TYPE dbo.THierarchy AS TABLE
(
	element_id INT NOT NULL, 
	sequenceNo [int] NULL, 
	parent_ID INT,
	[Object_ID] INT,
	NAME NVARCHAR(2000),
	StringValue NVARCHAR(MAX) NOT NULL,
	ValueType VARCHAR(10) NOT null 
	PRIMARY KEY (element_id)
)
GO

IF OBJECT_ID (N'dbo.parseJSON') IS NOT NULL 
	DROP FUNCTION dbo.parseJSON
GO

CREATE FUNCTION dbo.parseJSON( @JSON NVARCHAR(MAX))
	RETURNS @hierarchy TABLE
		  (
		   element_id INT IDENTITY(1, 1) NOT NULL, 
		   sequenceNo [int] NULL, 
		   parent_ID INT,
		   Object_ID INT,
		   NAME NVARCHAR(2000),
		   StringValue NVARCHAR(MAX) NOT NULL,
		   ValueType VARCHAR(10) NOT null 
		  )
		AS
		BEGIN
		  DECLARE
		    @FirstObject INT, 
		    @OpenDelimiter INT,
		    @NextOpenDelimiter INT,
		    @NextCloseDelimiter INT,
		    @Type NVARCHAR(10),
		    @NextCloseDelimiterChar CHAR(1),
		    @Contents NVARCHAR(MAX),
		    @Start INT, 
		    @end INT,
		    @param INT,
		    @EndOfName INT,
		    @token NVARCHAR(200),
		    @value NVARCHAR(MAX),
		    @SequenceNo int,
		    @name NVARCHAR(200),
		    @parent_ID INT,
		    @lenJSON INT,
		    @characters NCHAR(36),
		    @result BIGINT,
		    @index SMALLINT,
		    @Escape INT
		    
		  DECLARE @Strings TABLE 
		    (
		     String_ID INT IDENTITY(1, 1),
		     StringValue NVARCHAR(MAX)
		    )
		  SELECT
		    @characters='0123456789abcdefghijklmnopqrstuvwxyz',
		    @SequenceNo=0, 
		    @parent_ID=0;
		  WHILE 1=1 
		    BEGIN
		      SELECT
		        @start=PATINDEX('%[^a-zA-Z]["]%', @json collate SQL_Latin1_General_CP850_Bin);
		      IF @start=0 BREAK 
		      IF SUBSTRING(@json, @start+1, 1)='"' 
		        BEGIN
		          SET @start=@Start+1;
		          SET @end=PATINDEX('%[^\]["]%', RIGHT(@json, LEN(@json+'|')-@start) collate SQL_Latin1_General_CP850_Bin);
		        END
		      IF @end=0 
		        BREAK
		      SELECT @token=SUBSTRING(@json, @start+1, @end-1)
		      
		      SELECT @token=REPLACE(@token, FROMString, TOString)
		      FROM
		        (SELECT
		          '\"' AS FromString, '"' AS ToString
		         UNION ALL SELECT '\\', '\'
		         UNION ALL SELECT '\/', '/'
		         UNION ALL SELECT '\b', CHAR(08)
		         UNION ALL SELECT '\f', CHAR(12)
		         UNION ALL SELECT '\n', CHAR(10)
		         UNION ALL SELECT '\r', CHAR(13)
		         UNION ALL SELECT '\t', CHAR(09)
		        ) substitutions
		      SELECT @result=0, @escape=1
		      WHILE @escape>0
		        BEGIN
		          SELECT @index=0,
		          @escape=PATINDEX('%\x[0-9a-f][0-9a-f][0-9a-f][0-9a-f]%', @token collate SQL_Latin1_General_CP850_Bin)
		          IF @escape>0 
		            BEGIN
		              WHILE @index<4 
		                BEGIN
		                  SELECT 
		                    @result=@result+POWER(16, @index)
		                    *(CHARINDEX(SUBSTRING(@token, @escape+2+3-@index, 1),
		                                @characters)-1), @index=@index+1 ;
		         
		                END
		              SELECT @token=STUFF(@token, @escape, 6, NCHAR(@result))
		            END
		        END
		      INSERT INTO @Strings (StringValue) SELECT @token
		      SELECT @JSON=STUFF(@json, @start, @end+1,
		                    '@string'+CONVERT(NVARCHAR(5), @@identity))
		    END
		  WHILE 1=1 
		  BEGIN
		 
		  SELECT @parent_ID=@parent_ID+1
		  SELECT @FirstObject=PATINDEX('%[{[[]%', @json collate SQL_Latin1_General_CP850_Bin)
		  IF @FirstObject = 0 BREAK
		  IF (SUBSTRING(@json, @FirstObject, 1)='{') 
		    SELECT @NextCloseDelimiterChar='}', @type='object'
		  ELSE 
		    SELECT @NextCloseDelimiterChar=']', @type='array'
		  SELECT @OpenDelimiter=@firstObject
		  WHILE 1=1 
		    BEGIN
		      SELECT
		        @lenJSON=LEN(@JSON+'|')-1
		      SELECT
		        @NextCloseDelimiter=CHARINDEX(@NextCloseDelimiterChar, @json,
		                                      @OpenDelimiter+1)
		      SELECT @NextOpenDelimiter=PATINDEX('%[{[[]%',
		             RIGHT(@json, @lenJSON-@OpenDelimiter)collate SQL_Latin1_General_CP850_Bin)
		      IF @NextOpenDelimiter=0 
		        BREAK
		      SELECT @NextOpenDelimiter=@NextOpenDelimiter+@OpenDelimiter
		      IF @NextCloseDelimiter<@NextOpenDelimiter 
		        BREAK
		      IF SUBSTRING(@json, @NextOpenDelimiter, 1)='{' 
		        SELECT @NextCloseDelimiterChar='}', @type='object'
		      ELSE 
		        SELECT @NextCloseDelimiterChar=']', @type='array'
		      SELECT @OpenDelimiter=@NextOpenDelimiter
		    END
		  SELECT
		    @contents=SUBSTRING(@json, @OpenDelimiter+1,
		                        @NextCloseDelimiter-@OpenDelimiter-1)
		  SELECT
		    @JSON=STUFF(@json, @OpenDelimiter,
		                @NextCloseDelimiter-@OpenDelimiter+1,
		                '@'+@type+CONVERT(NVARCHAR(5), @parent_ID))
		  WHILE (PATINDEX('%[A-Za-z0-9@+.e]%', @contents collate SQL_Latin1_General_CP850_Bin))<>0 
		    BEGIN
		      IF @Type='Object'
		        BEGIN
		          SELECT
		            @SequenceNo=0,@end=CHARINDEX(':', ' '+@contents)
		          SELECT  @start=PATINDEX('%[^A-Za-z@][@]%', ' '+@contents collate SQL_Latin1_General_CP850_Bin)
		          SELECT @token=SUBSTRING(' '+@contents, @start+1, @End-@Start-1),
		            @endofname=PATINDEX('%[0-9]%', @token collate SQL_Latin1_General_CP850_Bin),
		            @param=RIGHT(@token, LEN(@token)-@endofname+1)
		          SELECT
		            @token=LEFT(@token, @endofname-1),
		            @Contents=RIGHT(' '+@contents, LEN(' '+@contents+'|')-@end-1)
		          SELECT  @name=stringvalue FROM @strings
		            WHERE string_id=@param
		        END
		      ELSE 
		        SELECT @Name=null,@SequenceNo=@SequenceNo+1 
		      SELECT
		        @end=CHARINDEX(',', @contents)
		      IF @end=0 
		        SELECT  @end=PATINDEX('%[A-Za-z0-9@+.e][^A-Za-z0-9@+.e]%', @Contents+' ' collate SQL_Latin1_General_CP850_Bin)
		          +1
		       SELECT
		        @start=PATINDEX('%[^A-Za-z0-9@+.e][A-Za-z0-9@+.e]%', ' '+@contents collate SQL_Latin1_General_CP850_Bin)
		      
		      SELECT
		        @Value=RTRIM(SUBSTRING(@contents, @start, @End-@Start)),
		        @Contents=RIGHT(@contents+' ', LEN(@contents+'|')-@end)
		      IF SUBSTRING(@value, 1, 7)='@object' 
		        INSERT INTO @hierarchy
		          (NAME, SequenceNo, parent_ID, StringValue, Object_ID, ValueType)
		          SELECT @name, @SequenceNo, @parent_ID, SUBSTRING(@value, 8, 5),
		            SUBSTRING(@value, 8, 5), 'object' 
		      ELSE 
		        IF SUBSTRING(@value, 1, 6)='@array' 
		          INSERT INTO @hierarchy
		            (NAME, SequenceNo, parent_ID, StringValue, Object_ID, ValueType)
		            SELECT @name, @SequenceNo, @parent_ID, SUBSTRING(@value, 7, 5),
		              SUBSTRING(@value, 7, 5), 'array' 
		        ELSE 
		          IF SUBSTRING(@value, 1, 7)='@string' 
		            INSERT INTO @hierarchy
		              (NAME, SequenceNo, parent_ID, StringValue, ValueType)
		              SELECT @name, @SequenceNo, @parent_ID, stringvalue, 'string'
		              FROM @strings
		              WHERE string_id=SUBSTRING(@value, 8, 5)
		          ELSE 
		            IF @value IN ('true', 'false') 
		              INSERT INTO @hierarchy
		                (NAME, SequenceNo, parent_ID, StringValue, ValueType)
		                SELECT @name, @SequenceNo, @parent_ID, @value, 'boolean'
		            ELSE
		              IF @value='null' 
		                INSERT INTO @hierarchy
		                  (NAME, SequenceNo, parent_ID, StringValue, ValueType)
		                  SELECT @name, @SequenceNo, @parent_ID, @value, 'null'
		              ELSE
		                IF PATINDEX('%[^0-9]%', @value collate SQL_Latin1_General_CP850_Bin)>0 
		                  INSERT INTO @hierarchy
		                    (NAME, SequenceNo, parent_ID, StringValue, ValueType)
		                    SELECT @name, @SequenceNo, @parent_ID, @value, 'real'
		                ELSE
		                  INSERT INTO @hierarchy
		                    (NAME, SequenceNo, parent_ID, StringValue, ValueType)
		                    SELECT @name, @SequenceNo, @parent_ID, @value, 'int'
		      if @Contents=' ' Select @SequenceNo=0
		    END
		  END
		INSERT INTO @hierarchy (NAME, SequenceNo, parent_ID, StringValue, Object_ID, ValueType)
		  SELECT '-',1, NULL, '', @parent_id-1, @type
		
		   RETURN
	END
Go

IF OBJECT_ID (N'dbo.JSONEscaped') IS NOT NULL  
	DROP FUNCTION dbo.JSONEscaped
GO

CREATE FUNCTION [dbo].[JSONEscaped](@Unescaped NVARCHAR(MAX))
RETURNS NVARCHAR(MAX)
AS 
BEGIN
	SELECT  @Unescaped = REPLACE(@Unescaped, FROMString, TOString)
	FROM    (SELECT
		    '\"' AS FromString, '"' AS ToString
		    UNION ALL SELECT '\', '\\'
		    UNION ALL SELECT '/', '\/'
		    UNION ALL SELECT  CHAR(08),'\b'
		    UNION ALL SELECT  CHAR(12),'\f'
		    UNION ALL SELECT  CHAR(10),'\n'
		    UNION ALL SELECT  CHAR(13),'\r'
		    UNION ALL SELECT  CHAR(09),'\t'
		    ) substitutions
	RETURN @Unescaped
END
GO

IF OBJECT_ID (N'dbo.ToJSON') IS NOT NULL 
	DROP FUNCTION dbo.ToJSON
GO

CREATE FUNCTION dbo.ToJSON(@Hierarchy THierarchy READONLY)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE
	@JSON NVARCHAR(MAX),
	@NewJSON NVARCHAR(MAX),
	@Where INT,
	@ANumber INT,
	@notNumber INT,
	@indent INT,
	@ii int,
	@CrLf CHAR(1)
	      
	SELECT @CrLf='',
	        @JSON = CASE ValueType WHEN 'array' THEN 
	        +COALESCE('{"'+NAME+'":','')+'[' 
	        ELSE '{' END
	        +@CrLf
	        + '@Object'+CONVERT(VARCHAR(5),OBJECT_ID)
	        +@CrLf+CASE ValueType WHEN 'array' THEN
	        case when NAME is null then ']' else ']}' end
	            ELSE '}' END
	FROM @Hierarchy 
	WHERE parent_id IS NULL AND valueType IN ('object','document','array')
	Select @ii=1000
	WHILE @ii>0
	begin
	SELECT @where= PATINDEX('%[^[a-zA-Z0-9]@Object%',@json)
	if @where=0 BREAK
	SET @indent=CHARINDEX('',Reverse(LEFT(@json,@where))+'')-1
	SET @NotNumber= PATINDEX('%[^0-9]%', RIGHT(@json,LEN(@JSON+'|')-@Where-8)+'')
	SET @NewJSON=NULL
	SELECT  
	    @NewJSON=COALESCE(@NewJSON+',','')
	    +case when parent.ValueType='array' then '' else COALESCE('"'+TheRow.NAME+'":','') end
	    +CASE TheRow.valuetype
	    WHEN 'array' THEN '['+@CrLf
	        +'@Object'+CONVERT(VARCHAR(5),TheRow.[OBJECT_ID])+@CrLf+']' 
	    WHEN 'object' then '{'+@CrLf
	        +'@Object'+CONVERT(VARCHAR(5),TheRow.[OBJECT_ID])+@CrLf+'}'
	    WHEN 'string' THEN '"'+dbo.JSONEscaped(TheRow.StringValue)+'"'
	    ELSE TheRow.StringValue
	    END 
	    FROM @Hierarchy TheRow 
	    inner join @hierarchy Parent
	    on parent.element_ID=TheRow.parent_ID
	    WHERE TheRow.parent_id= SUBSTRING(@JSON,@where+8, @Notnumber-1)
	Select @JSON=STUFF (@JSON, @where+1, 8+@NotNumber-1, @NewJSON),@ii=@ii-1
	end
	return REPLACE(@JSON, ' ', '')
end
GO

IF (TYPE_ID(N'dbo.THierarchy') IS NOT NULL AND OBJECT_ID (N'dbo.parseJSON') IS NOT NULL AND OBJECT_ID (N'dbo.JSONEscaped') IS NOT NULL AND OBJECT_ID (N'dbo.ToJSON') IS NOT NULL AND OBJECT_ID (N'dbo.PersonSetting') IS NOT NULL)
BEGIN
	declare @ResultTable table(
		[Id] [int] NOT NULL,
		[Value] [nvarchar](max) NULL
	)

	declare @SchoolYears TIntId,
	@AcadYears TInt32,
	@Hierarchy THierarchy,
	@SY_Obj_Id int,
	@SY_Count_Diff int,
	@Pointer int,
	@AcadId int,
	@Id int,
	@Value nvarchar(max)

	insert into @ResultTable
	SELECT 
		Id, Value 
	FROM 
		[dbo].[PersonSetting] 
	where 
		Value IS NOT NULL
		AND
		([Key] = 'classprofilepanoramasetting' OR [Key] = 'studentprofilepanoramasetting')
		
	declare ResultTableCursor cursor local fast_forward for
	select * from @ResultTable
	open ResultTableCursor;

	fetch next from ResultTableCursor into @Id, @Value
	while @@FETCH_STATUS = 0
	begin
		insert into @Hierarchy 
		select * from parseJSON(@Value)
		
		if exists(select [Object_ID] from @Hierarchy where NAME = 'SchoolYearIds')
			set @SY_Obj_Id = (select [Object_ID] from @Hierarchy where NAME = 'SchoolYearIds')
			insert into @SchoolYears 
			select StringValue from @Hierarchy where parent_ID = @SY_Obj_Id

		insert into @AcadYears
		select distinct AcadYear from SchoolYear where Id in (select Id from @SchoolYears)

		if exists(select * from @AcadYears)
		begin
			set @Pointer = 1
			set @SY_Count_Diff = (select count(*) from @Hierarchy where parent_ID = @SY_Obj_Id) - (select count(*) from @AcadYears)
					
			update @Hierarchy set element_id = element_id - @SY_Count_Diff
			update @Hierarchy set NAME = 'AcadYears' where Name = 'SchoolYearIds'
			delete from @Hierarchy where parent_ID = @SY_Obj_Id

			declare AcadCursor cursor local fast_forward for
			select * from @AcadYears
			open AcadCursor;
			fetch next from AcadCursor into @AcadId
			while @@FETCH_STATUS = 0
			begin
				insert into @Hierarchy ([element_id], [sequenceNo], [parent_Id], [Object_ID], [NAME], [StringValue], [ValueType])
				Values (@Pointer, @Pointer, @SY_Obj_Id, NULL, NULL, @AcadId, 'int')
				set @Pointer = @Pointer + 1
				fetch next from AcadCursor into @AcadId
			end
			UPDATE [dbo].[PersonSetting] SET [Value] = dbo.ToJSON(@Hierarchy) WHERE Id = @Id
			CLOSE AcadCursor;
			DEALLOCATE AcadCursor;
		end

		DELETE FROM @Hierarchy
		DELETE FROM @SchoolYears
		DELETE FROM @AcadYears
		fetch next from ResultTableCursor into @Id, @Value
	end
	CLOSE ResultTableCursor;
	DEALLOCATE ResultTableCursor;
END

GO

IF OBJECT_ID (N'dbo.parseJSON') IS NOT NULL 
	DROP FUNCTION dbo.parseJSON
GO

IF OBJECT_ID (N'dbo.JSONEscaped') IS NOT NULL  
	DROP FUNCTION dbo.JSONEscaped
GO

IF OBJECT_ID (N'dbo.ToJSON') IS NOT NULL 
	DROP FUNCTION dbo.ToJSON
GO

IF TYPE_ID(N'dbo.THierarchy') IS NOT NULL
	DROP TYPE dbo.THierarchy
GO