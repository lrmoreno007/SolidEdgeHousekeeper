Partial Class Form1
    Private Function PartFailedOrWarnedFeatures(
        ByVal SEDoc As SolidEdgePart.PartDocument) As List(Of String)

        Dim Models As SolidEdgePart.Models
        Dim Model As SolidEdgePart.Model
        Dim Features As SolidEdgePart.Features
        Dim Status As SolidEdgeConstants.FeatureStatusConstants

        Dim ErrorMessageList As New List(Of String)
        Dim ExitStatus As String = "0"
        Dim ErrorMessage As String = ""

        'igFeatureOK = 1216476310
        'igFeatureFailed = 1216476311
        'igFeatureWarned = 1216476312
        'igFeatureSuppressed = 1216476313
        'igFeatureRolledBack = 1216476314

        Models = SEDoc.Models

        If (Models.Count > 0) And (Models.Count < 10) Then
            For Each Model In Models
                Features = Model.Features
                'Some Sync part features don't have a Status field.
                Try
                    For i As Integer = 0 To Features.Count - 1
                        If Not Features(i).DisplayName.Contains("CopyConstruction") Then
                            Status = Features(i).Status
                            ' MsgBox(Status.ToString)
                            If Status.ToString = "igFeatureFailed" Then
                                ExitStatus = "1"
                                ErrorMessage += "  " + Features(i).DisplayName + Chr(13)
                            End If
                            If Status.ToString = "igFeatureWarned" Then
                                ExitStatus = "1"
                                ErrorMessage += "  " + Features(i).DisplayName + Chr(13)
                            End If
                        End If
                    Next
                Catch ex As Exception
                End Try
            Next
        ElseIf Models.Count >= 10 Then
            ExitStatus = "1"
            ErrorMessage += "  " + Models.Count.ToString + " models in file exceeds maximum to process" + Chr(13)
        End If

        ErrorMessageList.Add(ExitStatus)
        ErrorMessageList.Add(ErrorMessage)
        Return ErrorMessageList
    End Function

    Private Function PartSuppressedOrRolledBackFeatures(
        ByVal SEDoc As SolidEdgePart.PartDocument) As List(Of String)

        Dim ErrorMessageList As New List(Of String)
        Dim ExitStatus As String = "0"
        Dim ErrorMessage As String = ""

        Dim Models As SolidEdgePart.Models
        Dim Model As SolidEdgePart.Model
        Dim Features As SolidEdgePart.Features
        Dim Status As SolidEdgeConstants.FeatureStatusConstants

        Models = SEDoc.Models

        If (Models.Count > 0) And (Models.Count < 10) Then
            For Each Model In Models
                Features = Model.Features
                'Some Sync part features don't have a Status field.
                Try
                    For i As Integer = 0 To Features.Count - 1
                        Status = Features(i).Status
                        If Status.ToString = "igFeatureSuppressed" Then
                            ExitStatus = "1"
                            ErrorMessage += "  " + Features(i).DisplayName + Chr(13)
                        End If
                        If Status.ToString = "igFeatureRolledBack" Then
                            ExitStatus = "1"
                            ErrorMessage += "  " + Features(i).DisplayName + Chr(13)
                        End If
                    Next
                Catch ex As Exception
                End Try

            Next
        ElseIf Models.Count >= 10 Then
            ExitStatus = "1"
            ErrorMessage += "  " + Models.Count.ToString + " models in file exceeds maximum to process" + Chr(13)
        End If

        ErrorMessageList.Add(ExitStatus)
        ErrorMessageList.Add(ErrorMessage)
        Return ErrorMessageList

    End Function

    Private Function PartUnderconstrainedProfiles(
        ByVal SEDoc As SolidEdgePart.PartDocument
        ) As List(Of String)

        Dim ErrorMessageList As New List(Of String)
        Dim ExitStatus As String = "0"
        Dim ErrorMessage As String = ""

        Dim ProfileSets As SolidEdgePart.ProfileSets = SEDoc.ProfileSets
        Dim ProfileSet As SolidEdgePart.ProfileSet

        If SEDoc.ModelingMode.ToString = "seModelingModeOrdered" Then
            For Each ProfileSet In ProfileSets
                If ProfileSet.IsUnderDefined Then
                    ExitStatus = "1"
                End If
            Next
        End If

        ErrorMessageList.Add(ExitStatus)
        ErrorMessageList.Add(ErrorMessage)
        Return ErrorMessageList
    End Function

    Private Function PartMaterialNotInMaterialTable(
        ByVal SEDoc As SolidEdgePart.PartDocument) As List(Of String)

        Dim ErrorMessageList As New List(Of String)
        Dim ExitStatus As String = "0"
        Dim ErrorMessage As String = ""

        Dim MatTable As SolidEdgeFramework.MatTable

        Dim MaterialLibList As Object = Nothing
        Dim NumMaterialLibraries As Integer
        Dim MaterialList As Object = Nothing
        Dim NumMaterials As Integer

        Dim MatTableProps As Array
        Dim MatTableProp As SolidEdgeConstants.MatTablePropIndex
        Dim DocPropValue As Object = Nothing
        Dim LibPropValue As Object = Nothing

        Dim ActiveMaterialLibrary As String = System.IO.Path.GetFileNameWithoutExtension(TextBoxActiveMaterialLibrary.Text)
        Dim ActiveMaterialLibraryPresent As Boolean = False
        Dim CurrentMaterialName As String = ""
        Dim MatTableMaterial As Object
        Dim CurrentMaterialNameInLibrary As Boolean = False
        Dim CurrentMaterialMatchesLibMaterial As Boolean = True

        Dim msg As String

        Dim Models As SolidEdgePart.Models

        Models = SEDoc.Models

        If Models.Count > 0 Then

            MatTable = SEApp.GetMaterialTable()
            MatTable.GetCurrentMaterialName(SEDoc, CurrentMaterialName)
            MatTable.GetMaterialLibraryList(MaterialLibList, NumMaterialLibraries)
            MatTableProps = System.Enum.GetValues(GetType(SolidEdgeConstants.MatTablePropIndex))

            'Make sure the ActiveMaterialLibrary in settings.txt is present
            For Each MatTableMaterial In MaterialLibList
                If MatTableMaterial.ToString = ActiveMaterialLibrary Then
                    ActiveMaterialLibraryPresent = True
                    Exit For
                End If
            Next

            If Not ActiveMaterialLibraryPresent Then
                msg = "ActiveMaterialLibrary " + TextBoxActiveMaterialLibrary.Text + " not found.  Exiting..." + Chr(13)
                msg += "Please update the Material Table on the Options tab." + Chr(13)
                MsgBox(msg)
                SEApp.Quit()
                End
            End If

            'See if the CurrentMaterialName is in the ActiveLibrary
            MatTable.GetMaterialListFromLibrary(ActiveMaterialLibrary, NumMaterials, MaterialList)
            For Each MatTableMaterial In MaterialList
                If MatTableMaterial.ToString.ToLower.Trim = CurrentMaterialName.ToLower.Trim Then
                    CurrentMaterialNameInLibrary = True

                    'The names match.  Check if their properties do, too.
                    MatTableProps = System.Enum.GetValues(GetType(SolidEdgeConstants.MatTablePropIndex))
                    For Each MatTableProp In MatTableProps
                        MatTable.GetMaterialPropValueFromLibrary(MatTableMaterial.ToString, ActiveMaterialLibrary, MatTableProp, LibPropValue)
                        MatTable.GetMaterialPropValueFromDoc(SEDoc, MatTableProp, DocPropValue)
                        If DocPropValue <> LibPropValue Then
                            If (DocPropValue.GetType = GetType(Double)) And (DocPropValue.GetType = GetType(Double)) Then
                                If Not Math.Abs(DocPropValue - LibPropValue) < 0.001 Then
                                    CurrentMaterialMatchesLibMaterial = False
                                    Exit For
                                End If
                            End If
                        End If
                        DocPropValue = Nothing
                        LibPropValue = Nothing
                    Next

                    If Not CurrentMaterialMatchesLibMaterial Then
                        MatTable.ApplyMaterialToDoc(SEDoc, MatTableMaterial.ToString, ActiveMaterialLibrary)
                        SEDoc.Save()
                        SEApp.DoIdle()
                        Exit For
                    End If
                End If
                If CurrentMaterialNameInLibrary Then
                    Exit For
                End If
            Next

            If Not CurrentMaterialNameInLibrary Then
                ExitStatus = "1"
                If CurrentMaterialName = "" Then
                    ErrorMessage = "    Material " + "'None'" + " not in " + ActiveMaterialLibrary + Chr(13)
                Else
                    ErrorMessage = "    Material '" + CurrentMaterialName + "' not in " + ActiveMaterialLibrary + Chr(13)
                End If
            End If
        End If

        ErrorMessageList.Add(ExitStatus)
        ErrorMessageList.Add(ErrorMessage)
        Return ErrorMessageList
    End Function

    Private Function PartPartNumberDoesNotMatchFilename(
        ByVal SEDoc As SolidEdgePart.PartDocument) As List(Of String)
        Dim ErrorMessageList As New List(Of String)
        Dim ExitStatus As String = "0"
        Dim ErrorMessage As String = ""

        Dim PropertySets As SolidEdgeFramework.PropertySets = Nothing
        Dim Properties As SolidEdgeFramework.Properties = Nothing
        Dim Prop As SolidEdgeFramework.Property = Nothing

        Dim PartNumber As String = ""
        Dim PartNumberPropertyFound As Boolean = False
        Dim TF As Boolean
        Dim Filename As String

        'Get the bare file name without directory information
        Filename = System.IO.Path.GetFileName(SEDoc.FullName)

        Dim msg As String = ""

        PropertySets = SEDoc.Properties

        For Each Properties In PropertySets
            msg += Properties.Name + Chr(13)
            For Each Prop In Properties
                'MsgBox(PartNumberPropertySet.ToLower + " " + Properties.Name.ToLower)
                TF = (ComboBoxPartNumberPropertySet.Text.ToLower = "custom") And (Properties.Name.ToLower = "custom")
                'MsgBox("")
                If TF Then
                    If Prop.Name = TextBoxPartNumberPropertyName.Text Then
                        PartNumber = Prop.Value.Trim
                        PartNumberPropertyFound = True
                        Exit For
                    End If
                Else
                    If Prop.Name = TextBoxPartNumberPropertyName.Text Then
                        PartNumber = Prop.Value.Trim
                        PartNumberPropertyFound = True
                        Exit For
                    End If
                End If
            Next
            If PartNumberPropertyFound Then
                Exit For
            End If
        Next

        If PartNumberPropertyFound Then
            If PartNumber.Trim = "" Then
                ExitStatus = "1"
                ErrorMessage = "    Part number not assigned" + Chr(13)
            End If
            If Not Filename.Contains(PartNumber) Then
                ExitStatus = "1"
                ErrorMessage = "    Part number '" + PartNumber
                ErrorMessage += "' not found in filename '" + Filename + "'" + Chr(13)
            End If
        Else
            ExitStatus = "1"
            ErrorMessage = "    PartNumberPropertyName: '" + TextBoxPartNumberPropertyName.Text + "'"
            ErrorMessage += " not found in PartNumberPropertySet: '" + ComboBoxPartNumberPropertySet.Text + "'" + Chr(13)
            If TextBoxPartNumberPropertyName.Text = "" Then
                ErrorMessage += "    Check the Options tab for valid entries"
            End If
        End If

        ErrorMessageList.Add(ExitStatus)
        ErrorMessageList.Add(ErrorMessage)
        Return ErrorMessageList
    End Function

    Private Function PartUpdateFaceAndViewStylesFromTemplate(
        ByVal SEDoc As SolidEdgePart.PartDocument) As List(Of String)

        Dim ErrorMessageList As New List(Of String)
        Dim ExitStatus As String = "0"
        Dim ErrorMessage As String = ""

        Dim SETemplateDoc As SolidEdgePart.PartDocument
        Dim Windows As SolidEdgeFramework.Windows
        Dim Window As SolidEdgeFramework.Window
        Dim View As SolidEdgeFramework.View
        Dim ViewStyles As SolidEdgeFramework.ViewStyles
        Dim ViewStyle As SolidEdgeFramework.ViewStyle

        Dim TemplateFilename As String = TextBoxTemplatePart.Text
        Dim TemplateActiveStyleName As String = ""
        Dim TempViewStyleName As String = ""
        Dim ViewStyleAlreadyPresent As Boolean
        Dim TemplateSkyboxName(5) As String
        Dim msg As String = ""

        SEDoc.ImportStyles(TemplateFilename, True)

        ' Find the active ViewStyle in the template file.
        SETemplateDoc = SEApp.Documents.Open(TemplateFilename)

        Windows = SETemplateDoc.Windows
        For Each Window In Windows
            View = Window.View
            TemplateActiveStyleName = View.Style.ToString
        Next

        ViewStyles = SETemplateDoc.ViewStyles

        For Each ViewStyle In ViewStyles
            If ViewStyle.StyleName = TemplateActiveStyleName Then
                For i As Integer = 0 To 5
                    TemplateSkyboxName(i) = ViewStyle.GetSkyboxSideFilename(i)
                Next
            End If
        Next

        SETemplateDoc.Close(False)
        SEApp.DoIdle()

        ' If a style by the same name exists in the target file, delete it.
        ViewStyleAlreadyPresent = False
        ViewStyles = SEDoc.ViewStyles
        For Each ViewStyle In ViewStyles
            If ViewStyle.StyleName = TemplateActiveStyleName Then
                ViewStyleAlreadyPresent = True
            Else
                TempViewStyleName = ViewStyle.StyleName
            End If
        Next

        System.Threading.Thread.Sleep(1000)
        SEApp.DoIdle()

        Windows = SEDoc.Windows

        If ViewStyleAlreadyPresent Then ' Hopefully deactivate the desired ViewStyle so it can be removed
            For Each Window In Windows
                View = Window.View
                View.Style = TempViewStyleName
            Next
            ViewStyles.Remove(TemplateActiveStyleName)
        End If

        ViewStyles.AddFromFile(TemplateFilename, TemplateActiveStyleName)

        For Each ViewStyle In ViewStyles
            If ViewStyle.StyleName = TemplateActiveStyleName Then
                ViewStyle.SkyboxType = SolidEdgeFramework.SeSkyboxType.seSkyboxTypeSkybox
                For i As Integer = 0 To 5
                    ViewStyle.SetSkyboxSideFilename(i, TemplateSkyboxName(i))
                Next
            End If
        Next

        For Each Window In Windows
            View = Window.View
            View.Style = TemplateActiveStyleName
        Next


        SEDoc.Save()
        SEApp.DoIdle()

        ErrorMessageList.Add(ExitStatus)
        ErrorMessageList.Add(ErrorMessage)
        Return ErrorMessageList

    End Function

    Private Function PartFitIsometricView(
        ByVal SEDoc As SolidEdgePart.PartDocument) As List(Of String)

        Dim RefPlanes As SolidEdgePart.RefPlanes
        Dim RefPlane As SolidEdgePart.RefPlane
        Dim Models As SolidEdgePart.Models

        Dim ErrorMessageList As New List(Of String)
        Dim ExitStatus As String = "0"
        Dim ErrorMessage As String = ""

        Models = SEDoc.Models

        If Models.Count > 0 Then
            RefPlanes = SEDoc.RefPlanes
            For Each RefPlane In RefPlanes
                RefPlane.Visible = False
            Next
        Else
            RefPlanes = SEDoc.RefPlanes
            For Each RefPlane In RefPlanes
                RefPlane.Visible = True
            Next
        End If

        'Some imported files crash on this command
        Try
            SEDoc.Constructions.Visible = False
        Catch ex As Exception
        End Try

        SEDoc.CoordinateSystems.Visible = False

        SEApp.StartCommand(SolidEdgeConstants.PartCommandConstants.PartViewISOView)
        SEApp.StartCommand(SolidEdgeConstants.PartCommandConstants.PartViewFit)

        SEDoc.Save()
        SEApp.DoIdle()

        ErrorMessageList.Add(ExitStatus)
        ErrorMessageList.Add(ErrorMessage)
        Return ErrorMessageList
    End Function


End Class