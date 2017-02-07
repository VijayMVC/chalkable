CREATE TYPE TAnnouncementOrder AS TABLE
(
	Id int,
	FilteredField sql_variant,
	SortedField sql_variant
)
