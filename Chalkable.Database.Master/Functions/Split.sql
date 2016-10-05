
CREATE function [dbo].[Split]
(
    @delimiter nvarchar(10),
	@string nvarchar(4000)
)
returns @table table
(
    [s] nvarchar(4000)
)
begin
    declare @nextString nvarchar(4000)
    declare @pos int, @nextPos int

    set @nextString = ''
    set @string = @string + @delimiter

    set @pos = charindex(@delimiter, @string)
    set @nextPos = 1
    while (@pos <> 0)
    begin
        set @nextString = substring(@string, 1, @pos - 1)

        insert into @table
        (
            [s]
        )
        values
        (
            @nextString
        )

        set @string = substring(@string, @pos + len(@delimiter), len(@string))
        set @nextPos = @pos
        set @pos = charindex(@delimiter, @string)
    end
    return
end