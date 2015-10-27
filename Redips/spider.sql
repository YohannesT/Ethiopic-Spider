Create Database NETAMDAT

On Primary
(
	  Name = 'NETAMDAT'
	, FileName = 'D:\Databases\netamdat\netamdat.mdf'
	, FileGrowth = 100
)

Go

Use NETAMDAT

Go

Create Schema web

Create Table web.Website
(
	  WebsiteID int not null primary key identity(1, 1)
	, URL nVARCHAR(MAX) not null
	, IPAddress nVARCHAR(1000)
	, Country nVARCHAR(200)
	, RobotsTxt nVARCHAR(MAX)
	, Date DateTime Not Null
)

Create Table web.WebPage
(
	  WebPageID  int not null primary key identity(1,1)
	, URL nVARCHAR(MAX) not null
	, WebsiteID int not null Foreign Key References Website(WebsiteID)
	, NavigatedFromWebPageID int Foreign Key References WebPage(WebPageID)
	, HtmlContent nVARCHAR(MAX) not null
	, TextContent nVARCHAR(MAX) not null
	, Date DateTime not null
	, NumberOfVisits int 
)

--Drop table word
Create Table web.EthiopicWord
(
	  EthiopicWordID int not null primary key identity(1,1)
	, Name nVARCHAR(600) not null
	, SourceWebPageID int  not null  Foreign Key References WebPage(WebPageID)
	, Date DateTime not null
)

Create Table web.SeedWebsite
(
	  SeedWebsiteID int Primary Key Identity (1, 1)
	, URL nVARCHAR(600) Not Null
	, IsActive Bit Not Null
	, IsVisited Bit Not Null
	, Username nVARCHAR(100)
	, Password nVARCHAR(100)
	, CrawlDelayInMinutes int
	, VisitCount int
)

Create View web.WebPageView	As
	Select wp.WebPageID
	, wp.URL
	, wp.Date [Visited Date]
	, len(wp.HtmlContent) [HTML Length]
	, len(wp.TextContent) [Text Length]
	, Count(ew.Name) [Ethiopic Word Count]
	, SUBSTRING(wp.HtmlContent, 0, 100) [HTML Sample]
	, SUBSTRING(wp.TextContent, 0, 100) [Text Sample]
	From web.Website w
	join web.webPage wp on w.websiteid = wp.websiteid
	join web.EthiopicWord ew on wp.webpageid = ew.sourcewebpageid
	Group By wp.WebPageID, wp.URL, wp.Date, wp.HTMLContent, wp.TextContent
	Order by [Visited Date] desc


	/*
	delete from web.EthiopicWord where sourcewebpageid in (1085, 1084, 1154, 1155)
	delete from web.webpage where webpageid in (1085, 1084, 1154, 1155)
	*/