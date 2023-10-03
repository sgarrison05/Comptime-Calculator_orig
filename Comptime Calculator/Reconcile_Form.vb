﻿
Public Class frm_Reconcile

    Dim path As String = "C:\Comptime\comptimerun.txt"
    Dim path2 As String = "C:\Comptime\" & CStr(Year(Today) - 1)
    Dim readtxt As String
    Dim entry As String
    Dim newLineIndex As Integer
    Dim entryIndex As Integer
    Dim searchyear As String
    Dim entrywyr As String
    Dim searchyearorig As String
    Dim temp As String

    '----------------------------- Events -------------------------------------------------

    Private Sub frm_Reconcile_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        'default selects the previous year
        Me.txtYear.Text = Year(Today) - 1

        'clear out variables for recalculation
        searchyear = 0
        readtxt = ""
        entry = ""
        entrywyr = ""
        newLineIndex = 0
        entryIndex = 0
        temp = ""

        'disable reconcile button prior to preview
        Me.btnReconcile.Enabled = False

        Me.libxReconcile.Items.Clear()

        'checks for existing comptimerun.txt
        'if it exists, it pulls it and stores it
        If My.Computer.FileSystem.FileExists(path) Then

            readtxt = My.Computer.FileSystem.ReadAllText(path)

            'saves copy of text on load
            temp = readtxt

            'primer for first read of readtxt
            newLineIndex = readtxt.IndexOf(ControlChars.NewLine, entryIndex)

            Do Until newLineIndex = -1

                'get each line
                entry = readtxt.Substring(entryIndex, newLineIndex - entryIndex)

                'finds every line with year you are searching for
                entrywyr = entry.Contains("/")

                'if found, it adds it to preview
                If entrywyr = True Then

                    libxReconcile.Items.Add(entry)

                End If

                'updates entryindex with next line
                entryIndex = newLineIndex + 2
                newLineIndex = readtxt.IndexOf(ControlChars.NewLine, entryIndex)

            Loop

            'gives year focus
            Me.txtYear.Focus()

        End If

    End Sub

    '--------------------- Buttons and Click Events ----------------------------------------

    Private Sub btnExit_Click(sender As System.Object, e As System.EventArgs) Handles btnExit.Click

        Me.Close()
        frm_Main.Show()

    End Sub

    Private Sub btnPreview_Click(sender As Object, e As EventArgs) Handles btnPreview.Click

        'Previews the Previous Year
        'clear out variables for recalculation
        searchyear = 0
        readtxt = ""
        entry = ""
        entrywyr = ""
        newLineIndex = 0
        entryIndex = 0
        temp = ""

        Me.libxPreview.Items.Clear()
        Me.libxOrig.Items.Clear()

        'parses year to search
        Integer.TryParse(Me.txtYear.Text, searchyear)

        ''clears data pull
        'Me.txtReconcile.Text = ""

        'checks for existing comptimerun.txt
        'if it exists, it pulls it and stores it
        If My.Computer.FileSystem.FileExists(path) Then
            readtxt = My.Computer.FileSystem.ReadAllText(path)

        End If

        'collects txt variable contents before any manipulation to data
        temp = readtxt

        'primer for first read
        newLineIndex = readtxt.IndexOf(ControlChars.NewLine, entryIndex)

        Do Until newLineIndex = -1

            'get each line
            entry = readtxt.Substring(entryIndex, newLineIndex - entryIndex)

            'finds every line with year you are searching for
            entrywyr = entry.Contains(searchyear)

            'if found, it adds it to preview
            If entrywyr = True Then
                libxPreview.Items.Add(entry)
            End If

            'updates entryindex with next line
            entryIndex = newLineIndex + 2
            newLineIndex = readtxt.IndexOf(ControlChars.NewLine, entryIndex)

        Loop

        'Previews the Current Year
        'resets variables for second pull of original data
        searchyearorig = 0
        entry = ""
        entrywyr = ""
        newLineIndex = 0
        entryIndex = 0

        'parses current year to search
        Integer.TryParse(Me.txtYear.Text, searchyearorig)

        'since default is the previous year, add one
        searchyearorig += 1

        'primer for first read
        newLineIndex = readtxt.IndexOf(ControlChars.NewLine, entryIndex)

        Do Until newLineIndex = -1

            'get each line
            entry = readtxt.Substring(entryIndex, newLineIndex - entryIndex)

            'finds every line with year you are searching for
            entrywyr = entry.Contains(searchyearorig)

            'if found, it adds it to preview
            If entrywyr = True Then
                libxOrig.Items.Add(entry)
            End If

            'updates entryindex with next line
            entryIndex = newLineIndex + 2
            newLineIndex = readtxt.IndexOf(ControlChars.NewLine, entryIndex)

        Loop

        're-enables reconcile button after preview selected
        Me.btnReconcile.Enabled = True

    End Sub

    Private Sub btnReconcile_Click(sender As Object, e As EventArgs) Handles btnReconcile.Click

        'Creates reconcilled text document for previous year 
        'in it's own folder

        If Me.libxOrig.Items.Count > 0 And Me.libxPreview.Items.Count > 0 Then

            'checks if file exists.  If is does not, it creates it.
            If My.Computer.FileSystem.FileExists(path2) And
                My.Computer.FileSystem.FileExists(path2 & "\Comptimerun_Reconciled_" & txtYear.Text.ToString & ".txt") Then
                My.Computer.FileSystem.WriteAllText(path2 &
                                                    "\Comptimerun_Reconciled_" & txtYear.Text.ToString & ".txt", String.Empty, False)

                My.Computer.FileSystem.WriteAllText(path2 & "\Comptimerun_Reconciled_" & txtYear.Text.ToString & ".txt",
                                                    "Orange County Juvenile Probation Dept." & ControlChars.NewLine &
                                                    "---------------------------------------" & ControlChars.NewLine &
                                                    "Personal Comptime for " & frm_Main.user &
                                                    " for year " & txtYear.Text.ToString & ControlChars.NewLine & ControlChars.NewLine &
                                                    "Date Entered" & Strings.Space(7) &
                                                    "CaseNo." & Strings.Space(14) &
                                                    "Earned(+)" & Strings.Space(12) &
                                                    "Type" & Strings.Space(11) &
                                                    "Taken(-)" & Strings.Space(6) &
                                                    "Balance" & ControlChars.NewLine &
                                                    "-------------" & Strings.Space(5) &
                                                    "----------" & Strings.Space(11) &
                                                    "------------" & Strings.Space(9) &
                                                    "----------" & Strings.Space(5) &
                                                    "----------" & Strings.Space(4) &
                                                    "----------" & ControlChars.NewLine, False)
            Else
                My.Computer.FileSystem.CreateDirectory(path2)
                My.Computer.FileSystem.WriteAllText(path2 & "\Comptimerun_Reconciled_" & txtYear.Text.ToString & ".txt",
                                                    "Orange County Juvenile Probation Dept." & ControlChars.NewLine &
                                                    "---------------------------------------" & ControlChars.NewLine &
                                                    "Personal Comptime for " & frm_Main.user &
                                                    " for year " & txtYear.Text.ToString & ControlChars.NewLine & ControlChars.NewLine &
                                                    "Date Entered" & Strings.Space(7) &
                                                    "CaseNo." & Strings.Space(14) &
                                                    "Earned(+)" & Strings.Space(12) &
                                                    "Type" & Strings.Space(11) &
                                                    "Taken(-)" & Strings.Space(6) &
                                                    "Balance" & ControlChars.NewLine &
                                                    "--------------" & Strings.Space(5) &
                                                    "----------" & Strings.Space(11) &
                                                    "------------" & Strings.Space(9) &
                                                    "----------" & Strings.Space(5) &
                                                    "----------" & Strings.Space(4) &
                                                    "----------" & ControlChars.NewLine, False)
            End If

            'puts all items in text file
            For Each item As String In Me.libxPreview.Items
                My.Computer.FileSystem.WriteAllText(path2 & "\Comptimerun_Reconciled_" & txtYear.Text.ToString & ".txt",
                                                    item & ControlChars.NewLine, True)
                My.Computer.FileSystem.WriteAllText(path2 & "\Comptimerun_Reconciled_" & txtYear.Text.ToString & ".txt",
                                                    "".PadLeft(100, "-") & ControlChars.NewLine, True)

            Next item

            'Rebuilds current comptimerun text file
            If My.Computer.FileSystem.FileExists(path) Then
                My.Computer.FileSystem.WriteAllText(path, String.Empty, False)
                My.Computer.FileSystem.WriteAllText(path, "Orange County Juvenile Probation Dept." & ControlChars.NewLine &
                                                    "---------------------------------------" & ControlChars.NewLine &
                                                    "Personal Comptime Account for: " & frm_Main.user & ControlChars.NewLine & ControlChars.NewLine &
                                                    "Date Entered" & Strings.Space(7) &
                                                    "CaseNo." & Strings.Space(14) &
                                                    "Earned(+)" & Strings.Space(12) &
                                                    "Type" & Strings.Space(11) &
                                                    "Taken(-)" & Strings.Space(5) &
                                                    "Balance" & ControlChars.NewLine &
                                                    "-------------" & Strings.Space(6) &
                                                    "----------" & Strings.Space(11) &
                                                    "------------" & Strings.Space(9) &
                                                    "----------" & Strings.Space(5) &
                                                    "----------" & Strings.Space(4) &
                                                    "----------" & ControlChars.NewLine, False)

            Else

                My.Computer.FileSystem.WriteAllText(path, "Orange County Juvenile Probation Dept." & ControlChars.NewLine &
                                                    "---------------------------------------" & ControlChars.NewLine &
                                                    "Personal Comptime Account for: " & frm_Main.user & ControlChars.NewLine & ControlChars.NewLine &
                                                    "Date Entered" & Strings.Space(7) &
                                                    "CaseNo." & Strings.Space(14) &
                                                    "Earned(+)" & Strings.Space(12) &
                                                    "Type" & Strings.Space(11) &
                                                    "Taken(-)" & Strings.Space(6) &
                                                    "Balance" & ControlChars.NewLine &
                                                    "-------------" & Strings.Space(5) &
                                                    "----------" & Strings.Space(11) &
                                                    "------------" & Strings.Space(9) &
                                                    "----------" & Strings.Space(5) &
                                                    "----------" & Strings.Space(4) &
                                                    "----------" & ControlChars.NewLine, False)

            End If

            'puts all original items in comptimerun text file
            For Each item As String In Me.libxOrig.Items
                My.Computer.FileSystem.WriteAllText(path,
                                                    item & ControlChars.NewLine, True)

                My.Computer.FileSystem.WriteAllText(path,
                                                    "".PadLeft(100, "-") & ControlChars.NewLine, True)
            Next item

            'advises it is going to return to main form.
            MessageBox.Show("Reconcilliation Complete." & ControlChars.NewLine &
                            "The form will now close and return to the main form",
                            "Comptime Calculator", MessageBoxButtons.OK, MessageBoxIcon.Information)

            'if successful returns to main form
            Me.Close()
            frm_Main.Show()

        Else
            MessageBox.Show("Make sure you have a previous year to reconcile",
                            "Important", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

            Me.btnClearPrev.Focus()

        End If

    End Sub

    Private Sub btnClearPrev_Click(sender As Object, e As EventArgs) Handles btnClearPrev.Click

        Me.libxPreview.Items.Clear()
        Me.libxOrig.Items.Clear()

    End Sub

    '--------------------- Private Functions and Subroutines -------------------------------



    '---------------------- Individual Field Objects ---------------------------------------

    Private Sub txtYear_Enter(sender As Object, e As EventArgs) Handles txtYear.Enter

        Me.txtYear.SelectAll()

    End Sub

    Private Sub txtYear_TextChanged(sender As Object, e As EventArgs) Handles txtYear.TextChanged

        Me.libxPreview.Items.Clear()
        Me.libxOrig.Items.Clear()

    End Sub

End Class