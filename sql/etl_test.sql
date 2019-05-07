USE [EtlTest]
GO

/****** Object:  Table [dbo].[int32DataTypes]    Script Date: 3/23/2019 10:46:37 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

--drop table integer_test
CREATE TABLE [dbo].[integer_test](
	[a] [int] NULL,
	[b] [int] NULL,
	
) ON [PRIMARY]
GO

--drop table string_test
create table [dbo].[string_test]
(
[first_name] varchar(255),
[last_name] varchar(255),
[company_name] varchar(255),
[address] varchar(255),
[city] varchar(255),
[county] varchar(255),
[state] varchar(255),
[zip] varchar(255),
[phone1] varchar(255),
[phone2] varchar(255),
[email] varchar(255),
[web] varchar(255)
)

create table [dbo].[string_test2]
(
[first_name] varchar(255),
[last_name] varchar(255),
[company_name] varchar(255),
[address] varchar(255),
[city] varchar(255),
[county] varchar(255),
[state] varchar(255),
[zip] varchar(255),
[phone1] varchar(255),
[phone2] varchar(255),
[email] varchar(255),
[web] varchar(255)
)

select * from string_test
select * from string_test2

select * from integer_test