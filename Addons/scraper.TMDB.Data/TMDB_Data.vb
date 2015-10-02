﻿' ################################################################################
' #                             EMBER MEDIA MANAGER                              #
' ################################################################################
' ################################################################################
' # This file is part of Ember Media Manager.                                    #
' #                                                                              #
' # Ember Media Manager is free software: you can redistribute it and/or modify  #
' # it under the terms of the GNU General Public License as published by         #
' # the Free Software Foundation, either version 3 of the License, or            #
' # (at your option) any later version.                                          #
' #                                                                              #
' # Ember Media Manager is distributed in the hope that it will be useful,       #
' # but WITHOUT ANY WARRANTY; without even the implied warranty of               #
' # MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the                #
' # GNU General Public License for more details.                                 #
' #                                                                              #
' # You should have received a copy of the GNU General Public License            #
' # along with Ember Media Manager.  If not, see <http://www.gnu.org/licenses/>. #
' ################################################################################

Imports EmberAPI
Imports NLog

Public Class TMDB_Data
    Implements Interfaces.ScraperModule_Data_Movie
    Implements Interfaces.ScraperModule_Data_MovieSet
    Implements Interfaces.ScraperModule_Data_TV


#Region "Fields"

    Shared logger As Logger = NLog.LogManager.GetCurrentClassLogger()

    Public Shared _AssemblyName As String
    Public Shared ConfigScrapeOptions_Movie As New Structures.ScrapeOptions
    Public Shared ConfigScrapeOptions_MovieSet As New Structures.ScrapeOptions
    Public Shared ConfigScrapeOptions_TV As New Structures.ScrapeOptions
    Public Shared ConfigScrapeModifier_Movie As New Structures.ScrapeModifier
    Public Shared ConfigScrapeModifier_MovieSet As New Structures.ScrapeModifier
    Public Shared ConfigScrapeModifier_TV As New Structures.ScrapeModifier

    Private strPrivateAPIKey As String = String.Empty
    Private _SpecialSettings_Movie As New SpecialSettings
    Private _SpecialSettings_MovieSet As New SpecialSettings
    Private _SpecialSettings_TV As New SpecialSettings
    Private _Name As String = "TMDB_Data"
    Private _ScraperEnabled_Movie As Boolean = False
    Private _ScraperEnabled_MovieSet As Boolean = False
    Private _ScraperEnabled_TV As Boolean = False
    Private _setup_Movie As frmSettingsHolder_Movie
    Private _setup_MovieSet As frmSettingsHolder_MovieSet
    Private _setup_TV As frmSettingsHolder_TV

#End Region 'Fields

#Region "Events"

    'Movie part
    Public Event ModuleSettingsChanged_Movie() Implements Interfaces.ScraperModule_Data_Movie.ModuleSettingsChanged
    Public Event ScraperEvent_Movie(ByVal eType As Enums.ScraperEventType, ByVal Parameter As Object) Implements Interfaces.ScraperModule_Data_Movie.ScraperEvent
    Public Event ScraperSetupChanged_Movie(ByVal name As String, ByVal State As Boolean, ByVal difforder As Integer) Implements Interfaces.ScraperModule_Data_Movie.ScraperSetupChanged
    Public Event SetupNeedsRestart_Movie() Implements Interfaces.ScraperModule_Data_Movie.SetupNeedsRestart

    'MovieSet part
    Public Event ModuleSettingsChanged_MovieSet() Implements Interfaces.ScraperModule_Data_MovieSet.ModuleSettingsChanged
    Public Event ScraperEvent_MovieSet(ByVal eType As Enums.ScraperEventType, ByVal Parameter As Object) Implements Interfaces.ScraperModule_Data_MovieSet.ScraperEvent
    Public Event ScraperSetupChanged_MovieSet(ByVal name As String, ByVal State As Boolean, ByVal difforder As Integer) Implements Interfaces.ScraperModule_Data_MovieSet.ScraperSetupChanged
    Public Event SetupNeedsRestart_MovieSet() Implements Interfaces.ScraperModule_Data_MovieSet.SetupNeedsRestart

    'TV part
    Public Event ModuleSettingsChanged_TV() Implements Interfaces.ScraperModule_Data_TV.ModuleSettingsChanged
    Public Event ScraperEvent_TV(ByVal eType As Enums.ScraperEventType, ByVal Parameter As Object) Implements Interfaces.ScraperModule_Data_TV.ScraperEvent
    Public Event ScraperSetupChanged_TV(ByVal name As String, ByVal State As Boolean, ByVal difforder As Integer) Implements Interfaces.ScraperModule_Data_TV.ScraperSetupChanged
    Public Event SetupNeedsRestart_TV() Implements Interfaces.ScraperModule_Data_TV.SetupNeedsRestart

#End Region 'Events

#Region "Properties"

    ReadOnly Property ModuleName() As String Implements Interfaces.ScraperModule_Data_Movie.ModuleName, Interfaces.ScraperModule_Data_MovieSet.ModuleName, Interfaces.ScraperModule_Data_TV.ModuleName
        Get
            Return _Name
        End Get
    End Property

    ReadOnly Property ModuleVersion() As String Implements Interfaces.ScraperModule_Data_Movie.ModuleVersion, Interfaces.ScraperModule_Data_MovieSet.ModuleVersion, Interfaces.ScraperModule_Data_TV.ModuleVersion
        Get
            Return System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly.Location).FileVersion.ToString
        End Get
    End Property

    Property ScraperEnabled_Movie() As Boolean Implements Interfaces.ScraperModule_Data_Movie.ScraperEnabled
        Get
            Return _ScraperEnabled_Movie
        End Get
        Set(ByVal value As Boolean)
            _ScraperEnabled_Movie = value
        End Set
    End Property

    Property ScraperEnabled_MovieSet() As Boolean Implements Interfaces.ScraperModule_Data_MovieSet.ScraperEnabled
        Get
            Return _ScraperEnabled_MovieSet
        End Get
        Set(ByVal value As Boolean)
            _ScraperEnabled_MovieSet = value
        End Set
    End Property

    Property ScraperEnabled_TV() As Boolean Implements Interfaces.ScraperModule_Data_TV.ScraperEnabled
        Get
            Return _ScraperEnabled_TV
        End Get
        Set(ByVal value As Boolean)
            _ScraperEnabled_TV = value
        End Set
    End Property

#End Region 'Properties

#Region "Methods"

    Private Sub Handle_ModuleSettingsChanged_Movie()
        RaiseEvent ModuleSettingsChanged_Movie()
    End Sub

    Private Sub Handle_ModuleSettingsChanged_MovieSet()
        RaiseEvent ModuleSettingsChanged_MovieSet()
    End Sub

    Private Sub Handle_ModuleSettingsChanged_TV()
        RaiseEvent ModuleSettingsChanged_TV()
    End Sub

    Private Sub Handle_SetupNeedsRestart_Movie()
        RaiseEvent SetupNeedsRestart_Movie()
    End Sub

    Private Sub Handle_SetupNeedsRestart_MovieSet()
        RaiseEvent SetupNeedsRestart_MovieSet()
    End Sub

    Private Sub Handle_SetupNeedsRestart_TV()
        RaiseEvent SetupNeedsRestart_TV()
    End Sub

    Private Sub Handle_SetupScraperChanged_Movie(ByVal state As Boolean, ByVal difforder As Integer)
        ScraperEnabled_Movie = state
        RaiseEvent ScraperSetupChanged_Movie(String.Concat(Me._Name, "_Movie"), state, difforder)
    End Sub

    Private Sub Handle_SetupScraperChanged_MovieSet(ByVal state As Boolean, ByVal difforder As Integer)
        ScraperEnabled_MovieSet = state
        RaiseEvent ScraperSetupChanged_MovieSet(String.Concat(Me._Name, "_MovieSet"), state, difforder)
    End Sub

    Private Sub Handle_SetupScraperChanged_TV(ByVal state As Boolean, ByVal difforder As Integer)
        ScraperEnabled_TV = state
        RaiseEvent ScraperSetupChanged_TV(String.Concat(Me._Name, "_TV"), state, difforder)
    End Sub

    Sub Init_Movie(ByVal sAssemblyName As String) Implements Interfaces.ScraperModule_Data_Movie.Init
        _AssemblyName = sAssemblyName
        LoadSettings_Movie()
    End Sub

    Sub Init_MovieSet(ByVal sAssemblyName As String) Implements Interfaces.ScraperModule_Data_MovieSet.Init
        _AssemblyName = sAssemblyName
        LoadSettings_MovieSet()
    End Sub

    Sub Init_TV(ByVal sAssemblyName As String) Implements Interfaces.ScraperModule_Data_TV.Init
        _AssemblyName = sAssemblyName
        LoadSettings_TV()
    End Sub

    Function InjectSetupScraper_Movie() As Containers.SettingsPanel Implements Interfaces.ScraperModule_Data_Movie.InjectSetupScraper
        Dim SPanel As New Containers.SettingsPanel
        _setup_Movie = New frmSettingsHolder_Movie
        LoadSettings_Movie()
        _setup_Movie.chkEnabled.Checked = _ScraperEnabled_Movie
        _setup_Movie.chkCast.Checked = ConfigScrapeOptions_Movie.bMainActors
        _setup_Movie.chkCollectionID.Checked = ConfigScrapeOptions_Movie.bMainCollectionID
        _setup_Movie.chkCountry.Checked = ConfigScrapeOptions_Movie.bMainCountry
        _setup_Movie.chkDirector.Checked = ConfigScrapeOptions_Movie.bMainDirector
        _setup_Movie.chkFallBackEng.Checked = _SpecialSettings_Movie.FallBackEng
        _setup_Movie.chkGenre.Checked = ConfigScrapeOptions_Movie.bMainGenre
        _setup_Movie.chkGetAdultItems.Checked = _SpecialSettings_Movie.GetAdultItems
        _setup_Movie.chkCertification.Checked = ConfigScrapeOptions_Movie.bMainMPAA
        _setup_Movie.chkOriginalTitle.Checked = ConfigScrapeOptions_Movie.bMainOriginalTitle
        _setup_Movie.chkPlot.Checked = ConfigScrapeOptions_Movie.bMainPlot
        _setup_Movie.chkRating.Checked = ConfigScrapeOptions_Movie.bMainRating
        _setup_Movie.chkRelease.Checked = ConfigScrapeOptions_Movie.bMainRelease
        _setup_Movie.chkRuntime.Checked = ConfigScrapeOptions_Movie.bMainRuntime
        _setup_Movie.chkStudio.Checked = ConfigScrapeOptions_Movie.bMainStudio
        _setup_Movie.chkTagline.Checked = ConfigScrapeOptions_Movie.bMainTagline
        _setup_Movie.chkTitle.Checked = ConfigScrapeOptions_Movie.bMainTitle
        _setup_Movie.chkTrailer.Checked = ConfigScrapeOptions_Movie.bMainTrailer
        _setup_Movie.chkWriters.Checked = ConfigScrapeOptions_Movie.bMainWriters
        _setup_Movie.chkYear.Checked = ConfigScrapeOptions_Movie.bMainYear
        _setup_Movie.txtApiKey.Text = strPrivateAPIKey

        If Not String.IsNullOrEmpty(strPrivateAPIKey) Then
            _setup_Movie.btnUnlockAPI.Text = Master.eLang.GetString(443, "Use embedded API Key")
            _setup_Movie.lblEMMAPI.Visible = False
            _setup_Movie.txtApiKey.Enabled = True
        End If

        _setup_Movie.orderChanged()

        SPanel.Name = String.Concat(Me._Name, "_Movie")
        SPanel.Text = "TMDB"
        SPanel.Prefix = "TMDBMovieInfo_"
        SPanel.Order = 110
        SPanel.Parent = "pnlMovieData"
        SPanel.Type = Master.eLang.GetString(36, "Movies")
        SPanel.ImageIndex = If(_ScraperEnabled_Movie, 9, 10)
        SPanel.Panel = _setup_Movie.pnlSettings

        AddHandler _setup_Movie.SetupScraperChanged, AddressOf Handle_SetupScraperChanged_Movie
        AddHandler _setup_Movie.ModuleSettingsChanged, AddressOf Handle_ModuleSettingsChanged_Movie
        AddHandler _setup_Movie.SetupNeedsRestart, AddressOf Handle_SetupNeedsRestart_Movie
        Return SPanel
    End Function

    Function InjectSetupScraper_MovieSet() As Containers.SettingsPanel Implements Interfaces.ScraperModule_Data_MovieSet.InjectSetupScraper
        Dim SPanel As New Containers.SettingsPanel
        _setup_MovieSet = New frmSettingsHolder_MovieSet
        LoadSettings_MovieSet()
        _setup_MovieSet.chkEnabled.Checked = _ScraperEnabled_MovieSet
        _setup_MovieSet.chkFallBackEng.Checked = _SpecialSettings_MovieSet.FallBackEng
        _setup_MovieSet.chkGetAdultItems.Checked = _SpecialSettings_MovieSet.GetAdultItems
        _setup_MovieSet.chkPlot.Checked = ConfigScrapeOptions_MovieSet.bMainPlot
        _setup_MovieSet.chkTitle.Checked = ConfigScrapeOptions_MovieSet.bMainTitle
        _setup_MovieSet.txtApiKey.Text = strPrivateAPIKey

        If Not String.IsNullOrEmpty(strPrivateAPIKey) Then
            _setup_MovieSet.btnUnlockAPI.Text = Master.eLang.GetString(443, "Use embedded API Key")
            _setup_MovieSet.lblEMMAPI.Visible = False
            _setup_MovieSet.txtApiKey.Enabled = True
        End If

        _setup_MovieSet.orderChanged()

        SPanel.Name = String.Concat(Me._Name, "_MovieSet")
        SPanel.Text = "TMDB"
        SPanel.Prefix = "TMDBMovieSetInfo_"
        SPanel.Order = 110
        SPanel.Parent = "pnlMovieSetData"
        SPanel.Type = Master.eLang.GetString(1203, "MovieSets")
        SPanel.ImageIndex = If(_ScraperEnabled_MovieSet, 9, 10)
        SPanel.Panel = _setup_MovieSet.pnlSettings

        AddHandler _setup_MovieSet.SetupScraperChanged, AddressOf Handle_SetupScraperChanged_MovieSet
        AddHandler _setup_MovieSet.ModuleSettingsChanged, AddressOf Handle_ModuleSettingsChanged_MovieSet
        AddHandler _setup_MovieSet.SetupNeedsRestart, AddressOf Handle_SetupNeedsRestart_MovieSet
        Return SPanel
    End Function

    Function InjectSetupScraper_TV() As Containers.SettingsPanel Implements Interfaces.ScraperModule_Data_TV.InjectSetupScraper
        Dim SPanel As New Containers.SettingsPanel
        _setup_TV = New frmSettingsHolder_TV
        LoadSettings_TV()
        _setup_TV.chkEnabled.Checked = _ScraperEnabled_TV
        _setup_TV.chkFallBackEng.Checked = _SpecialSettings_TV.FallBackEng
        _setup_TV.chkGetAdultItems.Checked = _SpecialSettings_TV.GetAdultItems
        _setup_TV.chkScraperEpisodeActors.Checked = ConfigScrapeOptions_TV.bEpisodeActors
        _setup_TV.chkScraperEpisodeAired.Checked = ConfigScrapeOptions_TV.bEpisodeAired
        _setup_TV.chkScraperEpisodeCredits.Checked = ConfigScrapeOptions_TV.bEpisodeCredits
        _setup_TV.chkScraperEpisodeDirector.Checked = ConfigScrapeOptions_TV.bEpisodeDirector
        _setup_TV.chkScraperEpisodeGuestStars.Checked = ConfigScrapeOptions_TV.bEpisodeGuestStars
        _setup_TV.chkScraperEpisodePlot.Checked = ConfigScrapeOptions_TV.bEpisodePlot
        _setup_TV.chkScraperEpisodeRating.Checked = ConfigScrapeOptions_TV.bEpisodeRating
        _setup_TV.chkScraperEpisodeTitle.Checked = ConfigScrapeOptions_TV.bEpisodeTitle
        _setup_TV.chkScraperShowActors.Checked = ConfigScrapeOptions_TV.bMainActors
        _setup_TV.chkScraperShowCert.Checked = ConfigScrapeOptions_TV.bMainCert
        _setup_TV.chkScraperShowCountry.Checked = ConfigScrapeOptions_TV.bMainCountry
        _setup_TV.chkScraperShowCreator.Checked = ConfigScrapeOptions_TV.bMainCreator
        _setup_TV.chkScraperShowGenre.Checked = ConfigScrapeOptions_TV.bMainGenre
        _setup_TV.chkScraperShowOriginalTitle.Checked = ConfigScrapeOptions_TV.bMainOriginalTitle
        _setup_TV.chkScraperShowPlot.Checked = ConfigScrapeOptions_TV.bMainPlot
        _setup_TV.chkScraperShowPremiered.Checked = ConfigScrapeOptions_TV.bMainPremiered
        _setup_TV.chkScraperShowRating.Checked = ConfigScrapeOptions_TV.bMainRating
        _setup_TV.chkScraperShowRuntime.Checked = ConfigScrapeOptions_TV.bMainRuntime
        _setup_TV.chkScraperShowStatus.Checked = ConfigScrapeOptions_TV.bMainStatus
        _setup_TV.chkScraperShowStudio.Checked = ConfigScrapeOptions_TV.bMainStudio
        _setup_TV.chkScraperShowTitle.Checked = ConfigScrapeOptions_TV.bMainTitle
        _setup_TV.txtApiKey.Text = strPrivateAPIKey

        If Not String.IsNullOrEmpty(strPrivateAPIKey) Then
            _setup_TV.btnUnlockAPI.Text = Master.eLang.GetString(443, "Use embedded API Key")
            _setup_TV.lblEMMAPI.Visible = False
            _setup_TV.txtApiKey.Enabled = True
        End If

        _setup_TV.orderChanged()

        SPanel.Name = String.Concat(Me._Name, "_TV")
        SPanel.Text = "TMDB"
        SPanel.Prefix = "TMDBTVInfo_"
        SPanel.Order = 110
        SPanel.Parent = "pnlTVData"
        SPanel.Type = Master.eLang.GetString(653, "TV Shows")
        SPanel.ImageIndex = If(_ScraperEnabled_TV, 9, 10)
        SPanel.Panel = _setup_TV.pnlSettings

        AddHandler _setup_TV.SetupScraperChanged, AddressOf Handle_SetupScraperChanged_TV
        AddHandler _setup_TV.ModuleSettingsChanged, AddressOf Handle_ModuleSettingsChanged_TV
        AddHandler _setup_TV.SetupNeedsRestart, AddressOf Handle_SetupNeedsRestart_TV
        Return SPanel
    End Function

    Sub LoadSettings_Movie()
        ConfigScrapeOptions_Movie.bMainActors = clsAdvancedSettings.GetBooleanSetting("DoCast", True, , Enums.ContentType.Movie)
        ConfigScrapeOptions_Movie.bMainCert = clsAdvancedSettings.GetBooleanSetting("DoCert", True, , Enums.ContentType.Movie)
        ConfigScrapeOptions_Movie.bMainCollectionID = clsAdvancedSettings.GetBooleanSetting("DoCollectionID", True, , Enums.ContentType.Movie)
        ConfigScrapeOptions_Movie.bMainCountry = clsAdvancedSettings.GetBooleanSetting("DoCountry", True, , Enums.ContentType.Movie)
        ConfigScrapeOptions_Movie.bMainDirector = clsAdvancedSettings.GetBooleanSetting("DoDirector", True, , Enums.ContentType.Movie)
        ConfigScrapeOptions_Movie.bMainFullCrew = clsAdvancedSettings.GetBooleanSetting("DoFullCrews", True, , Enums.ContentType.Movie)
        ConfigScrapeOptions_Movie.bMainFullCrew = clsAdvancedSettings.GetBooleanSetting("FullCrew", True, , Enums.ContentType.Movie)
        ConfigScrapeOptions_Movie.bMainGenre = clsAdvancedSettings.GetBooleanSetting("DoGenres", True, , Enums.ContentType.Movie)
        ConfigScrapeOptions_Movie.bMainMPAA = clsAdvancedSettings.GetBooleanSetting("DoMPAA", True, , Enums.ContentType.Movie)
        ConfigScrapeOptions_Movie.bMainOriginalTitle = clsAdvancedSettings.GetBooleanSetting("DoOriginalTitle", True, , Enums.ContentType.Movie)
        ConfigScrapeOptions_Movie.bMainMusicBy = clsAdvancedSettings.GetBooleanSetting("DoMusic", True, , Enums.ContentType.Movie)
        ConfigScrapeOptions_Movie.bMainOtherCrew = clsAdvancedSettings.GetBooleanSetting("DoOtherCrews", True, , Enums.ContentType.Movie)
        ConfigScrapeOptions_Movie.bMainOutline = clsAdvancedSettings.GetBooleanSetting("DoOutline", True, , Enums.ContentType.Movie)
        ConfigScrapeOptions_Movie.bMainPlot = clsAdvancedSettings.GetBooleanSetting("DoPlot", True, , Enums.ContentType.Movie)
        ConfigScrapeOptions_Movie.bMainProducers = clsAdvancedSettings.GetBooleanSetting("DoProducers", True, , Enums.ContentType.Movie)
        ConfigScrapeOptions_Movie.bMainRating = clsAdvancedSettings.GetBooleanSetting("DoRating", True, , Enums.ContentType.Movie)
        ConfigScrapeOptions_Movie.bMainRelease = clsAdvancedSettings.GetBooleanSetting("DoRelease", True, , Enums.ContentType.Movie)
        ConfigScrapeOptions_Movie.bMainRuntime = clsAdvancedSettings.GetBooleanSetting("DoRuntime", True, , Enums.ContentType.Movie)
        ConfigScrapeOptions_Movie.bMainStudio = clsAdvancedSettings.GetBooleanSetting("DoStudio", True, , Enums.ContentType.Movie)
        ConfigScrapeOptions_Movie.bMainTagline = clsAdvancedSettings.GetBooleanSetting("DoTagline", True, , Enums.ContentType.Movie)
        ConfigScrapeOptions_Movie.bMainTitle = clsAdvancedSettings.GetBooleanSetting("DoTitle", True, , Enums.ContentType.Movie)
        ConfigScrapeOptions_Movie.bMainTop250 = clsAdvancedSettings.GetBooleanSetting("DoTop250", True, , Enums.ContentType.Movie)
        ConfigScrapeOptions_Movie.bMainTrailer = clsAdvancedSettings.GetBooleanSetting("DoTrailer", True, , Enums.ContentType.Movie)
        ConfigScrapeOptions_Movie.bMainWriters = clsAdvancedSettings.GetBooleanSetting("DoWriters", True, , Enums.ContentType.Movie)
        ConfigScrapeOptions_Movie.bMainYear = clsAdvancedSettings.GetBooleanSetting("DoYear", True, , Enums.ContentType.Movie)

        strPrivateAPIKey = clsAdvancedSettings.GetSetting("APIKey", String.Empty, , Enums.ContentType.Movie)
        _SpecialSettings_Movie.FallBackEng = clsAdvancedSettings.GetBooleanSetting("FallBackEn", False, , Enums.ContentType.Movie)
        _SpecialSettings_Movie.GetAdultItems = clsAdvancedSettings.GetBooleanSetting("GetAdultItems", False, , Enums.ContentType.Movie)
        _SpecialSettings_Movie.APIKey = If(String.IsNullOrEmpty(strPrivateAPIKey), "44810eefccd9cb1fa1d57e7b0d67b08d", strPrivateAPIKey)
    End Sub

    Sub LoadSettings_MovieSet()
        ConfigScrapeOptions_MovieSet.bMainPlot = clsAdvancedSettings.GetBooleanSetting("DoPlot", True, , Enums.ContentType.MovieSet)
        ConfigScrapeOptions_MovieSet.bMainTitle = clsAdvancedSettings.GetBooleanSetting("DoTitle", True, , Enums.ContentType.MovieSet)

        strPrivateAPIKey = clsAdvancedSettings.GetSetting("APIKey", String.Empty, , Enums.ContentType.MovieSet)
        _SpecialSettings_MovieSet.FallBackEng = clsAdvancedSettings.GetBooleanSetting("FallBackEn", False, , Enums.ContentType.MovieSet)
        _SpecialSettings_MovieSet.GetAdultItems = clsAdvancedSettings.GetBooleanSetting("GetAdultItems", False, , Enums.ContentType.MovieSet)
        _SpecialSettings_MovieSet.APIKey = If(String.IsNullOrEmpty(strPrivateAPIKey), "44810eefccd9cb1fa1d57e7b0d67b08d", strPrivateAPIKey)
    End Sub

    Sub LoadSettings_TV()
        ConfigScrapeOptions_TV.bEpisodeActors = clsAdvancedSettings.GetBooleanSetting("DoActors", True, , Enums.ContentType.TVEpisode)
        ConfigScrapeOptions_TV.bEpisodeAired = clsAdvancedSettings.GetBooleanSetting("DoAired", True, , Enums.ContentType.TVEpisode)
        ConfigScrapeOptions_TV.bEpisodeCredits = clsAdvancedSettings.GetBooleanSetting("DoCredits", True, , Enums.ContentType.TVEpisode)
        ConfigScrapeOptions_TV.bEpisodeDirector = clsAdvancedSettings.GetBooleanSetting("DoDirector", True, , Enums.ContentType.TVEpisode)
        ConfigScrapeOptions_TV.bEpisodeGuestStars = clsAdvancedSettings.GetBooleanSetting("DoGuestStars", True, , Enums.ContentType.TVEpisode)
        ConfigScrapeOptions_TV.bEpisodePlot = clsAdvancedSettings.GetBooleanSetting("DoPlot", True, , Enums.ContentType.TVEpisode)
        ConfigScrapeOptions_TV.bEpisodeRating = clsAdvancedSettings.GetBooleanSetting("DoRating", True, , Enums.ContentType.TVEpisode)
        ConfigScrapeOptions_TV.bEpisodeTitle = clsAdvancedSettings.GetBooleanSetting("DoTitle", True, , Enums.ContentType.TVEpisode)
        ConfigScrapeOptions_TV.bMainActors = clsAdvancedSettings.GetBooleanSetting("DoActors", True, , Enums.ContentType.TVShow)
        ConfigScrapeOptions_TV.bMainCert = clsAdvancedSettings.GetBooleanSetting("DoCert", True, , Enums.ContentType.TVShow)
        ConfigScrapeOptions_TV.bMainCountry = clsAdvancedSettings.GetBooleanSetting("DoCountry", True, , Enums.ContentType.TVShow)
        ConfigScrapeOptions_TV.bMainCreator = clsAdvancedSettings.GetBooleanSetting("DoCreator", True, , Enums.ContentType.TVShow)
        ConfigScrapeOptions_TV.bMainEpisodeGuide = clsAdvancedSettings.GetBooleanSetting("DoEpisodeGuide", False, , Enums.ContentType.TVShow)
        ConfigScrapeOptions_TV.bMainGenre = clsAdvancedSettings.GetBooleanSetting("DoGenre", True, , Enums.ContentType.TVShow)
        ConfigScrapeOptions_TV.bMainOriginalTitle = clsAdvancedSettings.GetBooleanSetting("DoOriginalTitle", True, , Enums.ContentType.TVShow)
        ConfigScrapeOptions_TV.bMainPlot = clsAdvancedSettings.GetBooleanSetting("DoPlot", True, , Enums.ContentType.TVShow)
        ConfigScrapeOptions_TV.bMainPremiered = clsAdvancedSettings.GetBooleanSetting("DoPremiered", True, , Enums.ContentType.TVShow)
        ConfigScrapeOptions_TV.bMainRating = clsAdvancedSettings.GetBooleanSetting("DoRating", True, , Enums.ContentType.TVShow)
        ConfigScrapeOptions_TV.bMainRuntime = clsAdvancedSettings.GetBooleanSetting("DoRuntime", True, , Enums.ContentType.TVShow)
        ConfigScrapeOptions_TV.bMainStatus = clsAdvancedSettings.GetBooleanSetting("DoStatus", True, , Enums.ContentType.TVShow)
        ConfigScrapeOptions_TV.bMainStudio = clsAdvancedSettings.GetBooleanSetting("DoStudio", True, , Enums.ContentType.TVShow)
        ConfigScrapeOptions_TV.bMainTitle = clsAdvancedSettings.GetBooleanSetting("DoTitle", True, , Enums.ContentType.TVShow)

        strPrivateAPIKey = clsAdvancedSettings.GetSetting("APIKey", String.Empty, , Enums.ContentType.TV)
        _SpecialSettings_TV.FallBackEng = clsAdvancedSettings.GetBooleanSetting("FallBackEn", False, , Enums.ContentType.TV)
        _SpecialSettings_TV.GetAdultItems = clsAdvancedSettings.GetBooleanSetting("GetAdultItems", False, , Enums.ContentType.TV)
        _SpecialSettings_TV.APIKey = If(String.IsNullOrEmpty(strPrivateAPIKey), "44810eefccd9cb1fa1d57e7b0d67b08d", strPrivateAPIKey)
    End Sub

    Sub SaveSettings_Movie()
        Using settings = New clsAdvancedSettings()
            settings.SetBooleanSetting("DoCast", ConfigScrapeOptions_Movie.bMainActors, , , Enums.ContentType.Movie)
            settings.SetBooleanSetting("DoCert", ConfigScrapeOptions_Movie.bMainCert, , , Enums.ContentType.Movie)
            settings.SetBooleanSetting("DoCollectionID", ConfigScrapeOptions_Movie.bMainCollectionID, , , Enums.ContentType.Movie)
            settings.SetBooleanSetting("DoCountry", ConfigScrapeOptions_Movie.bMainCountry, , , Enums.ContentType.Movie)
            settings.SetBooleanSetting("DoDirector", ConfigScrapeOptions_Movie.bMainDirector, , , Enums.ContentType.Movie)
            settings.SetBooleanSetting("DoFanart", ConfigScrapeModifier_Movie.MainFanart, , , Enums.ContentType.Movie)
            settings.SetBooleanSetting("DoFullCrews", ConfigScrapeOptions_Movie.bMainFullCrew, , , Enums.ContentType.Movie)
            settings.SetBooleanSetting("DoGenres", ConfigScrapeOptions_Movie.bMainGenre, , , Enums.ContentType.Movie)
            settings.SetBooleanSetting("DoMPAA", ConfigScrapeOptions_Movie.bMainMPAA, , , Enums.ContentType.Movie)
            settings.SetBooleanSetting("DoOriginalTitle", ConfigScrapeOptions_Movie.bMainOriginalTitle, , , Enums.ContentType.Movie)
            settings.SetBooleanSetting("DoMusic", ConfigScrapeOptions_Movie.bMainMusicBy, , , Enums.ContentType.Movie)
            settings.SetBooleanSetting("DoOtherCrews", ConfigScrapeOptions_Movie.bMainOtherCrew, , , Enums.ContentType.Movie)
            settings.SetBooleanSetting("DoOutline", ConfigScrapeOptions_Movie.bMainOutline, , , Enums.ContentType.Movie)
            settings.SetBooleanSetting("DoPlot", ConfigScrapeOptions_Movie.bMainPlot, , , Enums.ContentType.Movie)
            settings.SetBooleanSetting("DoPoster", ConfigScrapeModifier_Movie.MainPoster, , , Enums.ContentType.Movie)
            settings.SetBooleanSetting("DoProducers", ConfigScrapeOptions_Movie.bMainProducers, , , Enums.ContentType.Movie)
            settings.SetBooleanSetting("DoRating", ConfigScrapeOptions_Movie.bMainRating, , , Enums.ContentType.Movie)
            settings.SetBooleanSetting("DoRelease", ConfigScrapeOptions_Movie.bMainRelease, , , Enums.ContentType.Movie)
            settings.SetBooleanSetting("DoRuntime", ConfigScrapeOptions_Movie.bMainRuntime, , , Enums.ContentType.Movie)
            settings.SetBooleanSetting("DoStudio", ConfigScrapeOptions_Movie.bMainStudio, , , Enums.ContentType.Movie)
            settings.SetBooleanSetting("DoTagline", ConfigScrapeOptions_Movie.bMainTagline, , , Enums.ContentType.Movie)
            settings.SetBooleanSetting("DoTitle", ConfigScrapeOptions_Movie.bMainTitle, , , Enums.ContentType.Movie)
            settings.SetBooleanSetting("DoTop250", ConfigScrapeOptions_Movie.bMainTop250, , , Enums.ContentType.Movie)
            settings.SetBooleanSetting("DoTrailer", ConfigScrapeOptions_Movie.bMainTrailer, , , Enums.ContentType.Movie)
            settings.SetBooleanSetting("DoWriters", ConfigScrapeOptions_Movie.bMainWriters, , , Enums.ContentType.Movie)
            settings.SetBooleanSetting("DoYear", ConfigScrapeOptions_Movie.bMainYear, , , Enums.ContentType.Movie)
            settings.SetBooleanSetting("FallBackEn", _SpecialSettings_Movie.FallBackEng, , , Enums.ContentType.Movie)
            settings.SetBooleanSetting("FullCrew", ConfigScrapeOptions_Movie.bMainFullCrew, , , Enums.ContentType.Movie)
            settings.SetBooleanSetting("GetAdultItems", _SpecialSettings_Movie.GetAdultItems, , , Enums.ContentType.Movie)
            settings.SetSetting("APIKey", _setup_Movie.txtApiKey.Text, , , Enums.ContentType.Movie)
        End Using
    End Sub

    Sub SaveSettings_MovieSet()
        Using settings = New clsAdvancedSettings()
            settings.SetBooleanSetting("DoPlot", ConfigScrapeOptions_MovieSet.bMainPlot, , , Enums.ContentType.MovieSet)
            settings.SetBooleanSetting("DoTitle", ConfigScrapeOptions_MovieSet.bMainTitle, , , Enums.ContentType.MovieSet)
            settings.SetBooleanSetting("GetAdultItems", _SpecialSettings_MovieSet.GetAdultItems, , , Enums.ContentType.MovieSet)
            settings.SetSetting("APIKey", _setup_MovieSet.txtApiKey.Text, , , Enums.ContentType.MovieSet)
        End Using
    End Sub

    Sub SaveSettings_TV()
        Using settings = New clsAdvancedSettings()
            settings.SetBooleanSetting("DoActors", ConfigScrapeOptions_TV.bEpisodeActors, , , Enums.ContentType.TVEpisode)
            settings.SetBooleanSetting("DoAired", ConfigScrapeOptions_TV.bEpisodeAired, , , Enums.ContentType.TVEpisode)
            settings.SetBooleanSetting("DoCredits", ConfigScrapeOptions_TV.bEpisodeCredits, , , Enums.ContentType.TVEpisode)
            settings.SetBooleanSetting("DoDirector", ConfigScrapeOptions_TV.bEpisodeDirector, , , Enums.ContentType.TVEpisode)
            settings.SetBooleanSetting("DoGuestStars", ConfigScrapeOptions_TV.bEpisodeGuestStars, , , Enums.ContentType.TVEpisode)
            settings.SetBooleanSetting("DoPlot", ConfigScrapeOptions_TV.bEpisodePlot, , , Enums.ContentType.TVEpisode)
            settings.SetBooleanSetting("DoRating", ConfigScrapeOptions_TV.bEpisodeRating, , , Enums.ContentType.TVEpisode)
            settings.SetBooleanSetting("DoTitle", ConfigScrapeOptions_TV.bEpisodeTitle, , , Enums.ContentType.TVEpisode)
            settings.SetBooleanSetting("DoActors", ConfigScrapeOptions_TV.bMainActors, , , Enums.ContentType.TVShow)
            settings.SetBooleanSetting("DoCert", ConfigScrapeOptions_TV.bMainCert, , , Enums.ContentType.TVShow)
            settings.SetBooleanSetting("DoCountry", ConfigScrapeOptions_TV.bMainCountry, , , Enums.ContentType.TVShow)
            settings.SetBooleanSetting("DoCreator", ConfigScrapeOptions_TV.bMainCreator, , , Enums.ContentType.TVShow)
            settings.SetBooleanSetting("DoEpisodeGuide", ConfigScrapeOptions_TV.bMainEpisodeGuide, , , Enums.ContentType.TVShow)
            settings.SetBooleanSetting("DoGenre", ConfigScrapeOptions_TV.bMainGenre, , , Enums.ContentType.TVShow)
            settings.SetBooleanSetting("DoOriginalTitle", ConfigScrapeOptions_TV.bMainOriginalTitle, , , Enums.ContentType.TVShow)
            settings.SetBooleanSetting("DoPlot", ConfigScrapeOptions_TV.bMainPlot, , , Enums.ContentType.TVShow)
            settings.SetBooleanSetting("DoPremiered", ConfigScrapeOptions_TV.bMainPremiered, , , Enums.ContentType.TVShow)
            settings.SetBooleanSetting("DoRating", ConfigScrapeOptions_TV.bMainRating, , , Enums.ContentType.TVShow)
            settings.SetBooleanSetting("DoStatus", ConfigScrapeOptions_TV.bMainStatus, , , Enums.ContentType.TVShow)
            settings.SetBooleanSetting("DoStudio", ConfigScrapeOptions_TV.bMainStudio, , , Enums.ContentType.TVShow)
            settings.SetBooleanSetting("DoTitle", ConfigScrapeOptions_TV.bMainTitle, , , Enums.ContentType.TVShow)
            settings.SetBooleanSetting("FallBackEn", _SpecialSettings_TV.FallBackEng, , , Enums.ContentType.TV)
            settings.SetBooleanSetting("GetAdultItems", _SpecialSettings_TV.GetAdultItems, , , Enums.ContentType.TV)
            settings.SetSetting("APIKey", _setup_TV.txtApiKey.Text, , , Enums.ContentType.TV)
        End Using
    End Sub

    Sub SaveSetupScraper_Movie(ByVal DoDispose As Boolean) Implements Interfaces.ScraperModule_Data_Movie.SaveSetupScraper
        ConfigScrapeOptions_Movie.bMainActors = _setup_Movie.chkCast.Checked
        ConfigScrapeOptions_Movie.bMainCert = _setup_Movie.chkCertification.Checked
        ConfigScrapeOptions_Movie.bMainCollectionID = _setup_Movie.chkCollectionID.Checked
        ConfigScrapeOptions_Movie.bMainCountry = _setup_Movie.chkCountry.Checked
        ConfigScrapeOptions_Movie.bMainDirector = _setup_Movie.chkDirector.Checked
        ConfigScrapeOptions_Movie.bMainFullCrew = True
        ConfigScrapeOptions_Movie.bMainGenre = _setup_Movie.chkGenre.Checked
        ConfigScrapeOptions_Movie.bMainMPAA = _setup_Movie.chkCertification.Checked
        ConfigScrapeOptions_Movie.bMainOriginalTitle = _setup_Movie.chkOriginalTitle.Checked
        ConfigScrapeOptions_Movie.bMainMusicBy = False
        ConfigScrapeOptions_Movie.bMainOtherCrew = False
        ConfigScrapeOptions_Movie.bMainOutline = _setup_Movie.chkPlot.Checked
        ConfigScrapeOptions_Movie.bMainPlot = _setup_Movie.chkPlot.Checked
        ConfigScrapeOptions_Movie.bMainProducers = _setup_Movie.chkDirector.Checked
        ConfigScrapeOptions_Movie.bMainRating = _setup_Movie.chkRating.Checked
        ConfigScrapeOptions_Movie.bMainRelease = _setup_Movie.chkRelease.Checked
        ConfigScrapeOptions_Movie.bMainRuntime = _setup_Movie.chkRuntime.Checked
        ConfigScrapeOptions_Movie.bMainStudio = _setup_Movie.chkStudio.Checked
        ConfigScrapeOptions_Movie.bMainTagline = _setup_Movie.chkTagline.Checked
        ConfigScrapeOptions_Movie.bMainTitle = _setup_Movie.chkTitle.Checked
        ConfigScrapeOptions_Movie.bMainTop250 = False
        ConfigScrapeOptions_Movie.bMainTrailer = _setup_Movie.chkTrailer.Checked
        ConfigScrapeOptions_Movie.bMainWriters = _setup_Movie.chkWriters.Checked
        ConfigScrapeOptions_Movie.bMainYear = _setup_Movie.chkYear.Checked
        _SpecialSettings_Movie.FallBackEng = _setup_Movie.chkFallBackEng.Checked
        _SpecialSettings_Movie.GetAdultItems = _setup_Movie.chkGetAdultItems.Checked
        SaveSettings_Movie()
        If DoDispose Then
            RemoveHandler _setup_Movie.SetupScraperChanged, AddressOf Handle_SetupScraperChanged_Movie
            RemoveHandler _setup_Movie.ModuleSettingsChanged, AddressOf Handle_ModuleSettingsChanged_Movie
            _setup_Movie.Dispose()
        End If
    End Sub

    Sub SaveSetupScraper_MovieSet(ByVal DoDispose As Boolean) Implements Interfaces.ScraperModule_Data_MovieSet.SaveSetupScraper
        ConfigScrapeOptions_MovieSet.bMainPlot = _setup_MovieSet.chkPlot.Checked
        ConfigScrapeOptions_MovieSet.bMainTitle = _setup_MovieSet.chkTitle.Checked
        _SpecialSettings_MovieSet.FallBackEng = _setup_MovieSet.chkFallBackEng.Checked
        _SpecialSettings_MovieSet.GetAdultItems = _setup_MovieSet.chkGetAdultItems.Checked
        SaveSettings_MovieSet()
        If DoDispose Then
            RemoveHandler _setup_MovieSet.SetupScraperChanged, AddressOf Handle_SetupScraperChanged_Movieset
            RemoveHandler _setup_MovieSet.ModuleSettingsChanged, AddressOf Handle_ModuleSettingsChanged_MovieSet
            _setup_MovieSet.Dispose()
        End If
    End Sub

    Sub SaveSetupScraper_TV(ByVal DoDispose As Boolean) Implements Interfaces.ScraperModule_Data_TV.SaveSetupScraper
        ConfigScrapeOptions_TV.bEpisodeActors = _setup_TV.chkScraperEpisodeActors.Checked
        ConfigScrapeOptions_TV.bEpisodeAired = _setup_TV.chkScraperEpisodeAired.Checked
        ConfigScrapeOptions_TV.bEpisodeCredits = _setup_TV.chkScraperEpisodeCredits.Checked
        ConfigScrapeOptions_TV.bEpisodeDirector = _setup_TV.chkScraperEpisodeDirector.Checked
        ConfigScrapeOptions_TV.bEpisodeGuestStars = _setup_TV.chkScraperEpisodeGuestStars.Checked
        ConfigScrapeOptions_TV.bEpisodePlot = _setup_TV.chkScraperEpisodePlot.Checked
        ConfigScrapeOptions_TV.bEpisodeRating = _setup_TV.chkScraperEpisodeRating.Checked
        ConfigScrapeOptions_TV.bEpisodeTitle = _setup_TV.chkScraperEpisodeTitle.Checked
        ConfigScrapeOptions_TV.bMainActors = _setup_TV.chkScraperShowActors.Checked
        ConfigScrapeOptions_TV.bMainCert = _setup_TV.chkScraperShowCert.Checked
        ConfigScrapeOptions_TV.bMainCreator = _setup_TV.chkScraperShowCreator.Checked
        ConfigScrapeOptions_TV.bMainGenre = _setup_TV.chkScraperShowGenre.Checked
        ConfigScrapeOptions_TV.bMainOriginalTitle = _setup_TV.chkScraperShowOriginalTitle.Checked
        ConfigScrapeOptions_TV.bMainPlot = _setup_TV.chkScraperShowPlot.Checked
        ConfigScrapeOptions_TV.bMainPremiered = _setup_TV.chkScraperShowPremiered.Checked
        ConfigScrapeOptions_TV.bMainRating = _setup_TV.chkScraperShowRating.Checked
        ConfigScrapeOptions_TV.bMainRuntime = _setup_TV.chkScraperShowRuntime.Checked
        ConfigScrapeOptions_TV.bMainStatus = _setup_TV.chkScraperShowStatus.Checked
        ConfigScrapeOptions_TV.bMainStudio = _setup_TV.chkScraperShowStudio.Checked
        ConfigScrapeOptions_TV.bMainTitle = _setup_TV.chkScraperShowTitle.Checked
        _SpecialSettings_TV.FallBackEng = _setup_TV.chkFallBackEng.Checked
        _SpecialSettings_TV.GetAdultItems = _setup_TV.chkGetAdultItems.Checked
        SaveSettings_TV()
        If DoDispose Then
            RemoveHandler _setup_TV.SetupScraperChanged, AddressOf Handle_SetupScraperChanged_TV
            RemoveHandler _setup_TV.ModuleSettingsChanged, AddressOf Handle_ModuleSettingsChanged_TV
            _setup_TV.Dispose()
        End If
    End Sub

    Public Function GetLangs(ByRef Langs As clsXMLTVDBLanguages) As Interfaces.ModuleResult Implements Interfaces.ScraperModule_Data_TV.GetLanguages
        Return New Interfaces.ModuleResult With {.breakChain = False}
    End Function

    Function GetMovieStudio(ByRef DBMovie As Database.DBElement, ByRef sStudio As List(Of String)) As Interfaces.ModuleResult Implements Interfaces.ScraperModule_Data_Movie.GetMovieStudio
        If (DBMovie.Movie Is Nothing OrElse (String.IsNullOrEmpty(DBMovie.Movie.IMDBID) AndAlso String.IsNullOrEmpty(DBMovie.Movie.TMDBID))) Then
            logger.Error("Attempting to get studio for undefined movie")
            Return New Interfaces.ModuleResult
        End If

        LoadSettings_Movie()

        Dim _scraper As New TMDB.Scraper(_SpecialSettings_Movie)

        If Not String.IsNullOrEmpty(DBMovie.Movie.ID) Then
            'IMDB-ID is available
            sStudio.AddRange(_scraper.GetMovieStudios(DBMovie.Movie.ID))
        ElseIf Not String.IsNullOrEmpty(DBMovie.Movie.TMDBID) Then
            'TMDB-ID is available
            sStudio.AddRange(_scraper.GetMovieStudios(DBMovie.Movie.TMDBID))
        End If

        logger.Trace("Finished TMDB Scraper")
        Return New Interfaces.ModuleResult With {.breakChain = False}
    End Function

    Function GetTMDBID(ByVal sIMDBID As String, ByRef sTMDBID As String) As Interfaces.ModuleResult Implements Interfaces.ScraperModule_Data_Movie.GetTMDBID
        If Not String.IsNullOrEmpty(sIMDBID) Then

            LoadSettings_Movie()

            Dim _scraper As New TMDB.Scraper(_SpecialSettings_Movie)

            sTMDBID = _scraper.GetMovieID(sIMDBID)
        End If
        Return New Interfaces.ModuleResult With {.breakChain = False}
    End Function

    Function GetCollectionID(ByVal sIMDBID As String, ByRef sCollectionID As String) As Interfaces.ModuleResult Implements Interfaces.ScraperModule_Data_MovieSet.GetCollectionID

        LoadSettings_MovieSet()

        Dim _scraper As New TMDB.Scraper(_SpecialSettings_MovieSet)

        sCollectionID = _scraper.GetMovieCollectionID(sIMDBID)

        If String.IsNullOrEmpty(sCollectionID) Then
            Return New Interfaces.ModuleResult With {.breakChain = False, .Cancelled = True}
        End If
        Return New Interfaces.ModuleResult With {.breakChain = False}
    End Function
    ''' <summary>
    '''  Scrape MovieDetails from TMDB
    ''' </summary>
    ''' <param name="DBMovie">Movie to be scraped. DBMovie as ByRef to use existing data for identifing movie and to fill with IMDB/TMDB ID for next scraper</param>
    ''' <param name="nMovie">New scraped movie data</param>
    ''' <param name="Options">What kind of data is being requested from the scrape(global scraper settings)</param>
    ''' <returns>Database.DBElement Object (nMovie) which contains the scraped data</returns>
    ''' <remarks></remarks>
    Function Scraper_Movie(ByRef oDBElement As Database.DBElement, ByRef nMovie As MediaContainers.Movie, ByRef ScrapeModifier As Structures.ScrapeModifier, ByRef ScrapeType As Enums.ScrapeType, ByRef ScrapeOptions As Structures.ScrapeOptions) As Interfaces.ModuleResult Implements Interfaces.ScraperModule_Data_Movie.Scraper
        logger.Trace("Started TMDB Scraper")

        LoadSettings_Movie()
        _SpecialSettings_Movie.PrefLanguage = oDBElement.Language

        Dim _scraper As New TMDB.Scraper(_SpecialSettings_Movie)

        Dim FilteredOptions As Structures.ScrapeOptions = Functions.ScrapeOptionsAndAlso(ScrapeOptions, ConfigScrapeOptions_Movie)

        If ScrapeModifier.MainNFO AndAlso Not ScrapeModifier.DoSearch Then
            If Not String.IsNullOrEmpty(oDBElement.Movie.ID) Then
                'IMDB-ID already available -> scrape and save data into an empty movie container (nMovie)
                _scraper.GetMovieInfo(oDBElement.Movie.ID, nMovie, FilteredOptions.bMainFullCrew, False, FilteredOptions, False)
            ElseIf Not String.IsNullOrEmpty(oDBElement.Movie.TMDBID) Then
                'TMDB-ID already available -> scrape and save data into an empty movie container (nMovie)
                _scraper.GetMovieInfo(oDBElement.Movie.TMDBID, nMovie, FilteredOptions.bMainFullCrew, False, FilteredOptions, False)
            ElseIf Not ScrapeType = Enums.ScrapeType.SingleScrape Then
                'no IMDB-ID or TMDB-ID for movie --> search first and try to get ID!
                If Not String.IsNullOrEmpty(oDBElement.Movie.Title) Then
                    _scraper.GetSearchMovieInfo(oDBElement.Movie.Title, oDBElement, nMovie, ScrapeType, FilteredOptions)
                End If
                'if still no ID retrieved -> exit
                If String.IsNullOrEmpty(nMovie.TMDBID) Then Return New Interfaces.ModuleResult With {.breakChain = False, .Cancelled = True}
            End If
        End If

        If String.IsNullOrEmpty(nMovie.TMDBID) Then
            Select Case ScrapeType
                Case Enums.ScrapeType.AllAuto, Enums.ScrapeType.FilterAuto, Enums.ScrapeType.MarkedAuto, Enums.ScrapeType.MissingAuto, Enums.ScrapeType.NewAuto, Enums.ScrapeType.SelectedAuto
                    nMovie = Nothing
                    Return New Interfaces.ModuleResult With {.breakChain = False}
            End Select
        End If

        If ScrapeType = Enums.ScrapeType.SingleScrape OrElse ScrapeType = Enums.ScrapeType.SingleAuto Then
            If String.IsNullOrEmpty(oDBElement.Movie.ID) AndAlso String.IsNullOrEmpty(oDBElement.Movie.TMDBID) Then
                Using dSearch As New dlgTMDBSearchResults_Movie(_SpecialSettings_Movie, _scraper)
                    If dSearch.ShowDialog(nMovie, oDBElement.Movie.Title, oDBElement.Filename, FilteredOptions, oDBElement.Movie.Year) = Windows.Forms.DialogResult.OK Then
                        _scraper.GetMovieInfo(nMovie.TMDBID, nMovie, FilteredOptions.bMainFullCrew, False, FilteredOptions, False)
                        'if a movie is found, set DoSearch back to "false" for following scrapers
                        ScrapeModifier.DoSearch = False
                    Else
                        nMovie = Nothing
                        Return New Interfaces.ModuleResult With {.breakChain = False, .Cancelled = True}
                    End If
                End Using
            End If
        End If

        'set new informations for following scrapers
        If nMovie IsNot Nothing Then
            If nMovie.TitleSpecified Then
                oDBElement.Movie.Title = nMovie.Title
            End If
            If nMovie.OriginalTitleSpecified Then
                oDBElement.Movie.OriginalTitle = nMovie.OriginalTitle
            End If
            If nMovie.YearSpecified Then
                oDBElement.Movie.Year = nMovie.Year
            End If
            If nMovie.IDSpecified Then
                oDBElement.Movie.ID = nMovie.ID
            End If
            If nMovie.IMDBIDSpecified Then
                oDBElement.Movie.IMDBID = nMovie.IMDBID
            End If
            If nMovie.TMDBIDSpecified Then
                oDBElement.Movie.TMDBID = nMovie.TMDBID
            End If
        End If

        logger.Trace("Finished TMDB Scraper")
        Return New Interfaces.ModuleResult With {.breakChain = False}
    End Function

    Function Scraper_MovieSet(ByRef oDBElement As Database.DBElement, ByRef nMovieSet As MediaContainers.MovieSet, ByRef ScrapeModifier As Structures.ScrapeModifier, ByRef ScrapeType As Enums.ScrapeType, ByRef ScrapeOptions As Structures.ScrapeOptions) As Interfaces.ModuleResult Implements Interfaces.ScraperModule_Data_MovieSet.Scraper
        logger.Trace("Started scrape TMDB")

        LoadSettings_MovieSet()
        _SpecialSettings_MovieSet.PrefLanguage = oDBElement.Language

        Dim _scraper As New TMDB.Scraper(_SpecialSettings_MovieSet)

        Dim FilteredOptions As Structures.ScrapeOptions = Functions.ScrapeOptionsAndAlso(ScrapeOptions, ConfigScrapeOptions_MovieSet)

        If ScrapeModifier.MainNFO AndAlso Not ScrapeModifier.DoSearch Then
            If Not String.IsNullOrEmpty(oDBElement.MovieSet.TMDB) Then
                'TMDB-ID already available -> scrape and save data into an empty movieset container (nMovieSet)
                _scraper.GetMovieSetInfo(oDBElement.MovieSet.TMDB, nMovieSet, False, FilteredOptions, False)
            ElseIf Not ScrapeType = Enums.ScrapeType.SingleScrape Then
                'no ITMDB-ID for movieset --> search first and try to get ID!
                If Not String.IsNullOrEmpty(oDBElement.MovieSet.Title) Then
                    _scraper.GetSearchMovieSetInfo(oDBElement.MovieSet.Title, oDBElement, nMovieSet, ScrapeType, FilteredOptions)
                End If
                'if still no ID retrieved -> exit
                If String.IsNullOrEmpty(nMovieSet.TMDB) Then Return New Interfaces.ModuleResult With {.breakChain = False, .Cancelled = True}
            End If
        End If

        If String.IsNullOrEmpty(nMovieSet.TMDB) Then
            Select Case ScrapeType
                Case Enums.ScrapeType.AllAuto, Enums.ScrapeType.FilterAuto, Enums.ScrapeType.MarkedAuto, Enums.ScrapeType.MissingAuto, Enums.ScrapeType.NewAuto, Enums.ScrapeType.SelectedAuto
                    nMovieSet = Nothing
                    Return New Interfaces.ModuleResult With {.breakChain = False}
            End Select
        End If

        If ScrapeType = Enums.ScrapeType.SingleScrape OrElse ScrapeType = Enums.ScrapeType.SingleAuto Then
            If String.IsNullOrEmpty(oDBElement.MovieSet.TMDB) Then
                Using dSearch As New dlgTMDBSearchResults_MovieSet(_SpecialSettings_MovieSet, _scraper)
                    If dSearch.ShowDialog(nMovieSet, oDBElement.MovieSet.Title, FilteredOptions) = Windows.Forms.DialogResult.OK Then
                        _scraper.GetMovieSetInfo(nMovieSet.TMDB, nMovieSet, False, FilteredOptions, False)
                        'if a movie is found, set DoSearch back to "false" for following scrapers
                        ScrapeModifier.DoSearch = False
                    Else
                        nMovieSet = Nothing
                        Return New Interfaces.ModuleResult With {.breakChain = False, .Cancelled = True}
                    End If
                End Using
            End If
        End If

        'set new informations for following scrapers

        If nMovieSet IsNot Nothing Then
            If nMovieSet.TitleSpecified Then
                oDBElement.MovieSet.Title = nMovieSet.Title
            End If
            If nMovieSet.TMDBSpecified Then
                oDBElement.MovieSet.TMDB = nMovieSet.TMDB
            End If
        End If

        logger.Trace("Finished TMDB Scraper")
        Return New Interfaces.ModuleResult With {.breakChain = False}
    End Function
    ''' <summary>
    '''  Scrape MovieDetails from TMDB
    ''' </summary>
    ''' <param name="oDBTV">TV Show to be scraped. DBTV as ByRef to use existing data for identifing tv show and to fill with IMDB/TMDB/TVDB ID for next scraper</param>
    ''' <param name="nShow">New scraped TV Show data</param>
    ''' <param name="Options">What kind of data is being requested from the scrape(global scraper settings)</param>
    ''' <returns>Database.DBElement Object (nMovie) which contains the scraped data</returns>
    ''' <remarks></remarks>
    Function Scraper_TV(ByRef oDBElement As Database.DBElement, ByRef nShow As MediaContainers.TVShow, ByRef ScrapeModifier As Structures.ScrapeModifier, ByRef ScrapeType As Enums.ScrapeType, ByRef ScrapeOptions As Structures.ScrapeOptions) As Interfaces.ModuleResult Implements Interfaces.ScraperModule_Data_TV.Scraper_TVShow
        logger.Trace("Started TMDB Scraper")

        LoadSettings_TV()
        _SpecialSettings_TV.PrefLanguage = oDBElement.Language

        Dim _scraper As New TMDB.Scraper(_SpecialSettings_TV)

        Dim FilteredOptions As Structures.ScrapeOptions = Functions.ScrapeOptionsAndAlso(ScrapeOptions, ConfigScrapeOptions_TV)

        If ScrapeModifier.MainNFO AndAlso Not ScrapeModifier.DoSearch Then
            If Not String.IsNullOrEmpty(oDBElement.TVShow.TMDB) Then
                'TMDB-ID already available -> scrape and save data into an empty tv show container (nShow)
                _scraper.GetTVShowInfo(oDBElement.TVShow.TMDB, nShow, False, FilteredOptions, False, ScrapeModifier.withEpisodes)
            ElseIf Not String.IsNullOrEmpty(oDBElement.TVShow.TVDB) Then
                oDBElement.TVShow.TMDB = _scraper.GetTMDBbyTVDB(oDBElement.TVShow.TVDB)
                If String.IsNullOrEmpty(oDBElement.TVShow.TMDB) Then Return New Interfaces.ModuleResult With {.breakChain = False, .Cancelled = True}
                _scraper.GetTVShowInfo(oDBElement.TVShow.TMDB, nShow, False, FilteredOptions, False, ScrapeModifier.withEpisodes)
            ElseIf Not String.IsNullOrEmpty(oDBElement.TVShow.IMDB) Then
                oDBElement.TVShow.TMDB = _scraper.GetTMDBbyIMDB(oDBElement.TVShow.IMDB)
                If String.IsNullOrEmpty(oDBElement.TVShow.TMDB) Then Return New Interfaces.ModuleResult With {.breakChain = False, .Cancelled = True}
                _scraper.GetTVShowInfo(oDBElement.TVShow.TMDB, nShow, False, FilteredOptions, False, ScrapeModifier.withEpisodes)
            ElseIf Not ScrapeType = Enums.ScrapeType.SingleScrape Then
                'no TVDB-ID for tv show --> search first and try to get ID!
                If Not String.IsNullOrEmpty(oDBElement.TVShow.Title) Then
                    _scraper.GetSearchTVShowInfo(oDBElement.TVShow.Title, oDBElement, nShow, ScrapeType, FilteredOptions)
                End If
                'if still no ID retrieved -> exit
                If String.IsNullOrEmpty(nShow.TMDB) Then Return New Interfaces.ModuleResult With {.breakChain = False, .Cancelled = True}
            End If
        End If

        If String.IsNullOrEmpty(nShow.TMDB) Then
            Select Case ScrapeType
                Case Enums.ScrapeType.AllAuto, Enums.ScrapeType.FilterAuto, Enums.ScrapeType.MarkedAuto, Enums.ScrapeType.MissingAuto, Enums.ScrapeType.NewAuto, Enums.ScrapeType.SelectedAuto
                    nShow = Nothing
                    Return New Interfaces.ModuleResult With {.breakChain = False}
            End Select
        End If

        If ScrapeType = Enums.ScrapeType.SingleScrape OrElse ScrapeType = Enums.ScrapeType.SingleAuto Then
            If String.IsNullOrEmpty(oDBElement.TVShow.TMDB) Then
                Using dSearch As New dlgTMDBSearchResults_TV(_SpecialSettings_TV, _scraper)
                    If dSearch.ShowDialog(nShow, oDBElement.TVShow.Title, oDBElement.ShowPath, FilteredOptions) = Windows.Forms.DialogResult.OK Then
                        _scraper.GetTVShowInfo(nShow.TMDB, nShow, False, FilteredOptions, False, ScrapeModifier.withEpisodes)
                        'if a tvshow is found, set DoSearch back to "false" for following scrapers
                        ScrapeModifier.DoSearch = False
                    Else
                        nShow = Nothing
                        Return New Interfaces.ModuleResult With {.breakChain = False, .Cancelled = True}
                    End If
                End Using
            End If
        End If

        'set new informations for following scrapers
        If nShow IsNot Nothing Then
            If nShow.TitleSpecified Then
                oDBElement.TVShow.Title = nShow.Title
            End If
            If nShow.TVDBSpecified Then
                oDBElement.TVShow.TVDB = nShow.TVDB
            End If
            If nShow.IMDBSpecified Then
                oDBElement.TVShow.IMDB = nShow.IMDB
            End If
            If nShow.TMDBSpecified Then
                oDBElement.TVShow.TMDB = nShow.TMDB
            End If
        End If

        logger.Trace("Finished TMDB Scraper")
        Return New Interfaces.ModuleResult With {.breakChain = False}
    End Function

    Public Function Scraper_TVEpisode(ByRef oDBElement As Database.DBElement, ByRef nEpisode As MediaContainers.EpisodeDetails, ByVal ScrapeOptions As Structures.ScrapeOptions) As Interfaces.ModuleResult Implements Interfaces.ScraperModule_Data_TV.Scraper_TVEpisode
        logger.Trace("Started TMDB Scraper")

        LoadSettings_TV()
        _SpecialSettings_TV.PrefLanguage = oDBElement.Language

        Dim _scraper As New TMDB.Scraper(_SpecialSettings_TV)

        Dim FilteredOptions As Structures.ScrapeOptions = Functions.ScrapeOptionsAndAlso(ScrapeOptions, ConfigScrapeOptions_TV)

        If String.IsNullOrEmpty(oDBElement.TVShow.TMDB) AndAlso Not String.IsNullOrEmpty(oDBElement.TVShow.TVDB) Then
            oDBElement.TVShow.TMDB = _scraper.GetTMDBbyTVDB(oDBElement.TVShow.TVDB)
        End If

        If Not String.IsNullOrEmpty(oDBElement.TVShow.TMDB) Then
            If Not oDBElement.TVEpisode.Episode = -1 AndAlso Not oDBElement.TVEpisode.Season = -1 Then
                nEpisode = _scraper.GetTVEpisodeInfo(CInt(oDBElement.TVShow.TMDB), oDBElement.TVEpisode.Season, oDBElement.TVEpisode.Episode, FilteredOptions)
            ElseIf Not String.IsNullOrEmpty(oDBElement.TVEpisode.Aired) Then
                nEpisode = _scraper.GetTVEpisodeInfo(CInt(oDBElement.TVShow.TMDB), oDBElement.TVEpisode.Aired, FilteredOptions)
            Else
                nEpisode = Nothing
            End If
        End If

        'set new informations for following scrapers
        If nEpisode IsNot Nothing Then
            If nEpisode.TitleSpecified Then
                oDBElement.TVEpisode.Title = nEpisode.Title
            End If
            If nEpisode.TVDBSpecified Then
                oDBElement.TVEpisode.TVDB = nEpisode.TVDB
            End If
            If nEpisode.IMDBSpecified Then
                oDBElement.TVEpisode.IMDB = nEpisode.IMDB
            End If
            If nEpisode.TMDBSpecified Then
                oDBElement.TVEpisode.TMDB = nEpisode.TMDB
            End If
        End If

        logger.Trace("Finished TMDB Scraper")
        Return New Interfaces.ModuleResult With {.breakChain = False}
    End Function

    Public Sub ScraperOrderChanged_Movie() Implements EmberAPI.Interfaces.ScraperModule_Data_Movie.ScraperOrderChanged
        _setup_Movie.orderChanged()
    End Sub

    Public Sub ScraperOrderChanged_MovieSet() Implements EmberAPI.Interfaces.ScraperModule_Data_MovieSet.ScraperOrderChanged
        _setup_MovieSet.orderChanged()
    End Sub

    Public Sub ScraperOrderChanged_TV() Implements EmberAPI.Interfaces.ScraperModule_Data_TV.ScraperOrderChanged
        _setup_TV.orderChanged()
    End Sub

#End Region 'Methods

#Region "Nested Types"

    Structure SpecialSettings

#Region "Fields"

        Dim FallBackEng As Boolean
        Dim GetAdultItems As Boolean
        Dim APIKey As String
        Dim PrefLanguage As String

#End Region 'Fields

    End Structure

#End Region 'Nested Types

End Class