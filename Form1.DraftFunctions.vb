Partial Class Form1
    Private Function DraftViewsMissingFiles(
        ByVal SEDoc As SolidEdgeDraft.DraftDocument
        ) As List(Of String)

        Dim ErrorMessageList As New List(Of String)
        Dim ExitStatus As String = "0"
        Dim ErrorMessage As String = ""

        Dim ModelLinks As SolidEdgeDraft.ModelLinks = Nothing
        Dim ModelLink As SolidEdgeDraft.ModelLink = Nothing

        Dim msg As String = ""

        ModelLinks = SEDoc.ModelLinks

        For Each ModelLink In ModelLinks
            If Not FileIO.FileSystem.FileExists(ModelLink.FileName) Then
                ExitStatus = "1"
                ErrorMessage += "    " + TruncateFullPath(ModelLink.FileName) + " not found" + Chr(13)
            End If
        Next

        ErrorMessageList.Add(ExitStatus)
        ErrorMessageList.Add(ErrorMessage)
        Return ErrorMessageList

    End Function

    Private Function DraftViewsOutOfDate(
        ByVal SEDoc As SolidEdgeDraft.DraftDocument) As List(Of String)

        Dim ErrorMessageList As New List(Of String)
        Dim ExitStatus As String = "0"
        Dim ErrorMessage As String = ""

        Dim Sections As SolidEdgeDraft.Sections = Nothing
        Dim Section As SolidEdgeDraft.Section = Nothing
        Dim SectionSheets As SolidEdgeDraft.SectionSheets = Nothing
        Dim Sheet As SolidEdgeDraft.Sheet = Nothing
        Dim DrawingViews As SolidEdgeDraft.DrawingViews = Nothing
        Dim DrawingView As SolidEdgeDraft.DrawingView = Nothing

        Sections = SEDoc.Sections
        Section = Sections.WorkingSection
        SectionSheets = Section.Sheets

        For Each Sheet In SectionSheets.OfType(Of SolidEdgeDraft.Sheet)()
            DrawingViews = Sheet.DrawingViews
            For Each DrawingView In DrawingViews.OfType(Of SolidEdgeDraft.DrawingView)()
                If Not DrawingView.IsUpToDate Then
                    ExitStatus = "1"
                    'ErrorMessage += "  " + Features(i).DisplayName + Chr(13)
                End If
            Next DrawingView

        Next Sheet

        ErrorMessageList.Add(ExitStatus)
        ErrorMessageList.Add(ErrorMessage)
        Return ErrorMessageList

    End Function

    Private Function DraftDetachedDimensionsOrAnnotations(
        ByVal SEDoc As SolidEdgeDraft.DraftDocument
        ) As List(Of String)

        Dim ErrorMessageList As New List(Of String)
        Dim ExitStatus As String = "0"
        Dim ErrorMessage As String = ""

        Dim msg As String = ""
        Dim DimensionErrorConstants As String = "125"
        Dim TF As Boolean

        Dim Variables As SolidEdgeFramework.Variables = Nothing
        Dim VariableList As SolidEdgeFramework.VariableList = Nothing
        Dim Variable As SolidEdgeFramework.variable = Nothing
        Dim Dimension As SolidEdgeFrameworkSupport.Dimension = Nothing

        Dim Balloons As SolidEdgeFrameworkSupport.Balloons
        Dim Balloon As SolidEdgeFrameworkSupport.Balloon

        Dim Sections As SolidEdgeDraft.Sections = Nothing
        Dim Section As SolidEdgeDraft.Section = Nothing
        Dim SectionSheets As SolidEdgeDraft.SectionSheets = Nothing
        Dim Sheet As SolidEdgeDraft.Sheet = Nothing
        Dim DrawingViews As SolidEdgeDraft.DrawingViews = Nothing
        Dim DrawingView As SolidEdgeDraft.DrawingView = Nothing

        Sections = SEDoc.Sections
        Section = Sections.WorkingSection
        SectionSheets = Section.Sheets

        For Each Sheet In SectionSheets.OfType(Of SolidEdgeDraft.Sheet)()
            Balloons = Sheet.Balloons
            For Each Balloon In Balloons
                'Doesn't always work
                Try
                    TF = Not Balloon.IsTerminatorAttachedToEntity
                    TF = TF And Balloon.VertexCount > 0
                    If TF Then
                        ExitStatus = "1"
                        ErrorMessage += "    " + Balloon.BalloonDisplayedText + Chr(13)
                    End If
                Catch ex As Exception
                End Try
            Next Balloon
        Next Sheet

        Variables = DirectCast(SEDoc.Variables, SolidEdgeFramework.Variables)

        ' Get a reference to the variablelist.
        VariableList = DirectCast(Variables.Query(pFindCriterium:="*",
                                  NamedBy:=SolidEdgeConstants.VariableNameBy.seVariableNameByBoth,
                                  VarType:=SolidEdgeConstants.VariableVarType.SeVariableVarTypeBoth),
                                  SolidEdgeFramework.VariableList)

        ' Process variables.
        For Each VariableListItem In VariableList.OfType(Of Object)()
            'Not all VariableListItem objects have a StatusOfDimension property.
            Try
                If DimensionErrorConstants.Contains(VariableListItem.StatusOfDimension.ToString) Then
                    ExitStatus = "1"
                    ErrorMessage += "    " + VariableListItem.DisplayName + Chr(13)
                End If
            Catch ex As Exception
            End Try
        Next

        ErrorMessageList.Add(ExitStatus)
        ErrorMessageList.Add(ErrorMessage)
        Return ErrorMessageList

    End Function

    Private Function DraftPartNumberDoesNotMatchFilename(
        ByVal SEDoc As SolidEdgeDraft.DraftDocument) As List(Of String)
        Dim ErrorMessageList As New List(Of String)
        Dim ExitStatus As String = "0"
        Dim ErrorMessage As String = ""

        Dim ModelLinks As SolidEdgeDraft.ModelLinks = Nothing
        Dim ModelLink As SolidEdgeDraft.ModelLink = Nothing
        Dim ModelLinkFilenames As New List(Of String)
        Dim ModelLinkFilename As String

        Dim PartNumber As String = ""
        Dim PartNumberPropertyFound As Boolean = False
        Dim PartNumberFound As Boolean
        Dim Filename As String

        'Get the bare file name without directory information or extension
        Filename = System.IO.Path.GetFileNameWithoutExtension(SEDoc.FullName)

        Dim msg As String = ""

        ModelLinks = SEDoc.ModelLinks

        For Each ModelLink In ModelLinks
            ModelLinkFilenames.Add(System.IO.Path.GetFileNameWithoutExtension(ModelLink.FileName))
        Next

        If ModelLinkFilenames.Count > 0 Then
            For Each ModelLinkFilename In ModelLinkFilenames
                If Filename = ModelLinkFilename Then
                    PartNumberFound = True
                End If
            Next
            If Not PartNumberFound Then
                ExitStatus = "1"
                ErrorMessage = "    Drawing file name '" + Filename + "'"
                ErrorMessage += " not the same as any model file name in the drawing:" + Chr(13)
                For Each ModelLinkFilename In ModelLinkFilenames
                    ErrorMessage += "    '" + ModelLinkFilename + "'" + Chr(13)
                Next
            End If
        End If

        ErrorMessageList.Add(ExitStatus)
        ErrorMessageList.Add(ErrorMessage)
        Return ErrorMessageList
    End Function

    Private Function DraftUpdateBackgroundFromTemplate(
        ByVal SEDoc As SolidEdgeDraft.DraftDocument
        ) As List(Of String)

        Dim ErrorMessageList As New List(Of String)
        Dim ExitStatus As String = "0"
        Dim ErrorMessage As String = ""

        Dim TemplateFilename As String = TextBoxTemplateDraft.Text
        Dim SETemplateDoc As SolidEdgeDraft.DraftDocument
        Dim Sections As SolidEdgeDraft.Sections
        Dim Section As SolidEdgeDraft.Section
        Dim SectionSheets As SolidEdgeDraft.SectionSheets
        Dim Sheet As SolidEdgeDraft.Sheet
        Dim TemplateSheetNames As New List(Of String)

        SETemplateDoc = SEApp.Documents.Open(TemplateFilename)

        Sections = SETemplateDoc.Sections
        Section = Sections.BackgroundSection
        SectionSheets = Section.Sheets

        For Each Sheet In SectionSheets
            TemplateSheetNames.Add(Sheet.Name)
        Next

        SETemplateDoc.Close()
        SEApp.DoIdle()

        Sections = SEDoc.Sections
        Section = Sections.BackgroundSection
        SectionSheets = Section.Sheets

        For Each Sheet In SectionSheets
            If TemplateSheetNames.Contains(Sheet.Name) Then
                Sheet.ReplaceBackground(TemplateFilename, Sheet.Name)
            Else
                ExitStatus = "1"
                ErrorMessage += "    Template has no background named '" + Sheet.Name + "'" + Chr(13)
            End If
        Next

        If ExitStatus = "0" Then
            SEDoc.Save()
            SEApp.DoIdle()
        End If

        ErrorMessageList.Add(ExitStatus)
        ErrorMessageList.Add(ErrorMessage)
        Return ErrorMessageList

    End Function

    Private Function DraftUpdateDimensionStylesFromTemplate(
        ByVal SEDoc As SolidEdgeDraft.DraftDocument
        ) As List(Of String)

        Dim ErrorMessageList As New List(Of String)
        Dim ExitStatus As String = "0"
        Dim ErrorMessage As String = ""

        Dim TemplateFilename As String = TextBoxTemplateDraft.Text
        Dim SETemplateDoc As SolidEdgeDraft.DraftDocument
        Dim DimensionStyles As SolidEdgeFrameworkSupport.DimensionStyles
        Dim DimensionStyle As SolidEdgeFrameworkSupport.DimensionStyle
        Dim ActiveDimensionStyle As SolidEdgeFrameworkSupport.DimensionStyle = Nothing
        Dim PrimaryDecimalRoundOffThisDimension As SolidEdgeFrameworkSupport.DimDecimalRoundOffTypeConstants
        Dim TextHeightThisDimension As Double
        Dim Dimensions As SolidEdgeFrameworkSupport.Dimensions = Nothing
        Dim Dimension As SolidEdgeFrameworkSupport.Dimension = Nothing

        Dim Balloons As SolidEdgeFrameworkSupport.Balloons
        Dim Balloon As SolidEdgeFrameworkSupport.Balloon

        Dim Sections As SolidEdgeDraft.Sections = Nothing
        Dim Section As SolidEdgeDraft.Section = Nothing
        Dim SectionSheets As SolidEdgeDraft.SectionSheets = Nothing
        Dim Sheet As SolidEdgeDraft.Sheet = Nothing

        'Copy DimensionStyles from template
        SETemplateDoc = SEApp.Documents.Open(TemplateFilename)

        DimensionStyles = SETemplateDoc.DimensionStyles

        For Each DimensionStyle In DimensionStyles
            SEDoc.DimensionStyles.AddEx(DimensionStyle.Name, True, SETemplateDoc)
        Next

        SETemplateDoc.Close()
        SEApp.DoIdle()

        'Update dimensions and callouts with overrides.  They are not automatically updated to the new style.
        Sections = SEDoc.Sections

        For Each Section In Sections
            SectionSheets = Section.Sheets
            For Each Sheet In SectionSheets
                Dimensions = Sheet.Dimensions
                For Each Dimension In Dimensions
                    For Each DimensionStyle In SEDoc.DimensionStyles
                        If DimensionStyle.Name = Dimension.Style.Name Then
                            ActiveDimensionStyle = DimensionStyle
                        End If
                    Next
                    If Dimension.Style.PrimaryDecimalRoundOff <> ActiveDimensionStyle.PrimaryDecimalRoundOff Then
                        PrimaryDecimalRoundOffThisDimension = Dimension.Style.PrimaryDecimalRoundOff
                        Dimension.Style.Name = ActiveDimensionStyle.Name
                        Dimension.Style.PrimaryDecimalRoundOff = PrimaryDecimalRoundOffThisDimension
                    End If
                Next
            Next
        Next

        For Each Section In Sections
            SectionSheets = Section.Sheets
            For Each Sheet In SectionSheets
                Balloons = Sheet.Balloons
                For Each Balloon In Balloons
                    For Each DimensionStyle In SEDoc.DimensionStyles
                        If DimensionStyle.Name = Balloon.Style.Name Then
                            ActiveDimensionStyle = DimensionStyle
                        End If
                    Next
                    If Balloon.Style.Height <> ActiveDimensionStyle.Height Then
                        TextHeightThisDimension = Balloon.Style.Height
                        Balloon.Style.Name = ActiveDimensionStyle.Name
                        Balloon.Style.Height = TextHeightThisDimension
                    Else
                        Balloon.Style.Name = ActiveDimensionStyle.Name
                    End If
                Next
            Next
        Next

        Section = Sections.WorkingSection

        SEDoc.Save()
        SEApp.DoIdle()

        ErrorMessageList.Add(ExitStatus)
        ErrorMessageList.Add(ErrorMessage)
        Return ErrorMessageList

    End Function

    Private Function DraftFitView(
        ByVal SEDoc As SolidEdgeDraft.DraftDocument) As List(Of String)

        Dim SheetWindow As SolidEdgeDraft.SheetWindow
        Dim Sections As SolidEdgeDraft.Sections = Nothing
        Dim Section As SolidEdgeDraft.Section = Nothing
        Dim SectionSheets As SolidEdgeDraft.SectionSheets = Nothing
        Dim Sheet As SolidEdgeDraft.Sheet = Nothing

        Dim ErrorMessageList As New List(Of String)
        Dim ExitStatus As String = "0"
        Dim ErrorMessage As String = ""

        Sections = SEDoc.Sections
        Section = Sections.WorkingSection
        SectionSheets = Section.Sheets

        SheetWindow = SEApp.ActiveWindow

        'Maximizes the window in the application
        If SheetWindow.WindowState <> 2 Then
            SheetWindow.WindowState = 2
            System.Threading.Thread.Sleep(1000)
            SEApp.DoIdle()
        End If

        For Each Sheet In SectionSheets.OfType(Of SolidEdgeDraft.Sheet)()
            SheetWindow.ActiveSheet = Sheet
            SheetWindow.FitEx(SolidEdgeDraft.SheetFitConstants.igFitSheet)
        Next

        SheetWindow.ActiveSheet = SectionSheets.OfType(Of SolidEdgeDraft.Sheet)().ElementAt(0)

        SEDoc.Save()
        SEApp.DoIdle()

        ErrorMessageList.Add(ExitStatus)
        ErrorMessageList.Add(ErrorMessage)
        Return ErrorMessageList
    End Function
    Private Function DraftSaveAsPDF(
        ByVal SEDoc As SolidEdgeDraft.DraftDocument) As List(Of String)
        Dim ErrorMessageList As New List(Of String)
        Dim ExitStatus As String = "0"
        Dim ErrorMessage As String = ""

        Dim PDFFilename As String = ""

        PDFFilename = System.IO.Path.ChangeExtension(SEDoc.FullName, ".pdf")

        'Capturing a fault to update ExitStatus
        Try
            SEDoc.SaveAs(PDFFilename)
            SEApp.DoIdle()
        Catch ex As Exception
            ExitStatus = "1"
            ErrorMessage = "    Error saving " + TruncateFullPath(PDFFilename)
        End Try

        ErrorMessageList.Add(ExitStatus)
        ErrorMessageList.Add(ErrorMessage)
        Return ErrorMessageList
    End Function

End Class