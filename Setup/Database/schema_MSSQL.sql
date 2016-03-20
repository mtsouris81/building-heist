USE [master]
GO
CREATE DATABASE [BuildingHeist]
GO
ALTER DATABASE [BuildingHeist] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [BuildingHeist].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [BuildingHeist] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [BuildingHeist] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [BuildingHeist] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [BuildingHeist] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [BuildingHeist] SET ARITHABORT OFF 
GO
ALTER DATABASE [BuildingHeist] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [BuildingHeist] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [BuildingHeist] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [BuildingHeist] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [BuildingHeist] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [BuildingHeist] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [BuildingHeist] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [BuildingHeist] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [BuildingHeist] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [BuildingHeist] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [BuildingHeist] SET  DISABLE_BROKER 
GO
ALTER DATABASE [BuildingHeist] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [BuildingHeist] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [BuildingHeist] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [BuildingHeist] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [BuildingHeist] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [BuildingHeist] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [BuildingHeist] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [BuildingHeist] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [BuildingHeist] SET  MULTI_USER 
GO
ALTER DATABASE [BuildingHeist] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [BuildingHeist] SET DB_CHAINING OFF 
GO
ALTER DATABASE [BuildingHeist] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [BuildingHeist] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [BuildingHeist] SET  READ_WRITE 
GO




USE [BuildingHeist]
GO
/****** Object:  Table [dbo].[Game]    Script Date: 2/29/2016 3:42:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Game](
	[dbId] [int] IDENTITY(1,1) NOT NULL,
	[Id] [nvarchar](255) NOT NULL,
	[Title] [nvarchar](120) NOT NULL,
	[Winner] [nvarchar](255) NULL,
 CONSTRAINT [PK_Game] PRIMARY KEY CLUSTERED 
(
	[dbId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[GamePlayers]    Script Date: 2/29/2016 3:42:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GamePlayers](
	[GameId] [int] NOT NULL,
	[PlayerId] [int] NOT NULL,
 CONSTRAINT [PK_GamePlayers] PRIMARY KEY CLUSTERED 
(
	[GameId] ASC,
	[PlayerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Player]    Script Date: 2/29/2016 3:42:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Player](
	[dbId] [int] IDENTITY(1,1) NOT NULL,
	[Id] [nvarchar](255) NOT NULL,
	[Username] [nvarchar](120) NOT NULL,
	[ProfileImage] [nvarchar](120) NULL,
	[Password] [nvarchar](120) NOT NULL,
	[Salt] [nvarchar](120) NOT NULL,
 CONSTRAINT [PK_Player] PRIMARY KEY CLUSTERED 
(
	[dbId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[GamePlayers]  WITH CHECK ADD  CONSTRAINT [FK_GamePlayers_Game] FOREIGN KEY([GameId])
REFERENCES [dbo].[Game] ([dbId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[GamePlayers] CHECK CONSTRAINT [FK_GamePlayers_Game]
GO
ALTER TABLE [dbo].[GamePlayers]  WITH CHECK ADD  CONSTRAINT [FK_GamePlayers_Player] FOREIGN KEY([PlayerId])
REFERENCES [dbo].[Player] ([dbId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[GamePlayers] CHECK CONSTRAINT [FK_GamePlayers_Player]
GO



CREATE TABLE [dbo].[PlayerFriends](
	[PlayerId_1] [int] NOT NULL,
	[PlayerId_2] [int] NOT NULL,
 CONSTRAINT [PK_PlayerFriends] PRIMARY KEY CLUSTERED 
(
	[PlayerId_1] ASC,
	[PlayerId_2] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[FriendRequests](
	[RequestingPlayerId] [int] NOT NULL,
	[PlayerId] [int] NOT NULL,
 CONSTRAINT [PK_FriendRequests] PRIMARY KEY CLUSTERED 
(
	[RequestingPlayerId] ASC,
	[PlayerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO






