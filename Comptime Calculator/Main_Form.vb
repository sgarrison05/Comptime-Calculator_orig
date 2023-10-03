'Title                  Comptime Calculator (Original Ver)
'Purpose                To calculate comptime time earned or spent
'                       in a particular instance
'Created By             Shon Garrison, December 2008
'Updated Last           October 2023

'Update Notes:          Added the ability to use straight or comp time.
'                       Before reformat
Option Explicit On

Public Class frm_Main

    Private Const cdirectory As String = "C:\Comptime"
    Private Const cpath As String = "C:\Comptime\comptimerun.txt"
    Private title As String = "Comptime Calculator"
    Public user As String
    Private newbalance As Decimal
    Private previous As Decimal
    Private myentry As String
    Private atime As String
    Private tlable As String
    Private heading As String = "Date Entered" & Strings.Space(7) &
                                "CaseNo." & Strings.Space(14) &
                                "Earned(+)" & Strings.Space(12) &
                                "Type" & Strings.Space(11) &
                                "Taken(-)" & Strings.Space(6) &
                                "Balance"

    Private Sub compcalcForm_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim my_decision As DialogResult

        Me.calcearnedTextBox.ReadOnly = True

        'disable apply calc button till preview is seen
        Me.btnApply.Enabled = False
        Me.ApplyToolStripMenuItem.Enabled = False

        'add items to case/reason combobox
        Me.caseComboBox.Items.Add("[Enter One]")
        Me.caseComboBox.Items.Add("Sick")
        Me.caseComboBox.Items.Add("Personal")
        Me.caseComboBox.Items.Add("Dr. Appt")
        Me.caseComboBox.Items.Add("New Case")
        Me.caseComboBox.Items.Add("Transport")
        Me.caseComboBox.Items.Add("Det Visit")
        Me.caseComboBox.Items.Add("On-Call")
        Me.caseComboBox.Items.Add("Spec Group")
        Me.caseComboBox.Items.Add("Plcmt Visit")
        Me.caseComboBox.Items.Add("Training")
        Me.caseComboBox.Items.Add("Evaluation")
        Me.caseComboBox.Items.Add("Meeting")

        'default selection
        Me.caseComboBox.SelectedItem = "[Enter One]"

        'add items to sctComboBox
        Me.sctComboBox.Items.Add("Straight Time (X 1.0)")
        Me.sctComboBox.Items.Add("Comp Time (X 1.5)")
        Me.sctComboBox.Items.Add("n/a")

        'default selection
        Me.sctComboBox.SelectedItem = "Comp Time (X 1.5)"

        'sets up a default selection for the radio buttons
        accruedRadioButton.Select()

        'sets focus to dateTimePicker
        Me.accruedDateTimePicker.Focus()

        'Verify txt file exists else ask if the user wants to create it.
        If My.Computer.FileSystem.FileExists(cpath) Then

            'pulls user variable from text file
            'declare block variables
            Dim readtxt As String
            Dim entry As String
            Dim newLineIndex As Integer = 0
            Dim entryIndex As Integer = 0
            Dim entryuser As String

            'checks for existing comptimerun.txt
            'if it exists, it pulls it and stores it
            If My.Computer.FileSystem.FileExists(cpath) Then

                readtxt = My.Computer.FileSystem.ReadAllText(cpath)

                'primer for first read of readtxt
                newLineIndex = readtxt.IndexOf(ControlChars.NewLine, entryIndex)

                Do Until newLineIndex = -1

                    'get each line
                    entry = readtxt.Substring(entryIndex, newLineIndex - entryIndex)

                    'finds line  with username that you are searching for
                    entryuser = entry.Contains("Account")

                    'if line is found with a date, add to myentry variable to find bank balance at the end
                    If entry.Contains("/") Then
                        myentry = Trim(Microsoft.VisualBasic.Right(entry, 7))
                    End If

                    'Retrieve Current Bank balance
                    prevbalLabel.Text = myentry

                    'if user is found, it adds it to preview
                    If entryuser = True Then
                        user = entry.Substring(31)
                    End If

                    'if not found updates entryindex with next line
                    entryIndex = newLineIndex + 2
                    newLineIndex = readtxt.IndexOf(ControlChars.NewLine, entryIndex)

                Loop

                Me.Text = "Personal Comptime Calculator for " & user
                newbalLabel.Text = "0.00"
                calcearnedTextBox.Text = "Ready"

                MyWarning()

            End If

        Else 'sets up the comptime back in the specified path
            my_decision = MessageBox.Show _
            ("The current comptime balance file does not exist.  This is your comptime bank, would you like to create it?",
            "Comptime Calculator", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)

            If my_decision = DialogResult.Yes Then
                Dim init_bal_decision As DialogResult
                init_bal_decision = MessageBox.Show("Do you have an current balance to enter?", title, MessageBoxButtons.YesNo,
                MessageBoxIcon.Question)

                If init_bal_decision = Windows.Forms.DialogResult.Yes Then
                    Do Until IsNumeric(prevbalLabel.Text) Or prevbalLabel.Text <> String.Empty
                        prevbalLabel.Text = InputBox("Please enter current balance or click 'Ok' to go to calculator.", title, "0.00")
                        If Not IsNumeric(prevbalLabel.Text) Then
                            MessageBox.Show("Number must be numeric.", title, MessageBoxButtons.OK)
                        End If
                    Loop

                    user = InputBox("Please Enter Your name", title, )

                    Me.Show()
                    Me.Text = "Personal Comptime Calculator for " & user
                    newbalLabel.Text = "0.00"
                    calcearnedTextBox.Text = "Ready"

                    MyWarning()

                ElseIf init_bal_decision = Windows.Forms.DialogResult.No Then
                    Me.Show()
                    prevbalLabel.Text = "0.00"
                    newbalLabel.Text = "0.00"
                    calcearnedTextBox.Text = "Ready"

                    Me.warningLbl.Hide()
                    Me.prevbalLabel.ForeColor = Color.Black

                End If

            Else : my_decision = DialogResult.No
                Me.Close()
            End If
        End If

    End Sub

    '------------------------------- Private Functions and Subroutines -----------------------------------------

    Public Sub ApplyCalculations()
        'Saves current balance to txt file

        Dim my_apply As DialogResult
        Dim my_another As DialogResult

        'It will ask to you append file.
        my_apply = MessageBox.Show("Do you wish to add to new balance to bank?",
                                   title, MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If my_apply = Windows.Forms.DialogResult.Yes Then

            'make calculations
            newbalance = Convert.ToDecimal(newbalLabel.Text)
            newbalance = Math.Round(newbalance, 2)
            previous = Convert.ToDecimal(prevbalLabel.Text)
            previous += newbalance
            previous = Math.Round(previous, 2)
            prevbalLabel.Text = Convert.ToString(previous)

            'Declare text writing variables
            Dim line As String
            Dim caseno As String
            Dim curdate As String

            'Convert the data from the Previous Balance Label and store it in line variable
            line = Convert.ToString(previous)
            caseno = caseComboBox.Text
            curdate = accruedDateTimePicker.Text

            MyLabels()

            'If Comptime file exists, the prog writes current balance text file
            If My.Computer.FileSystem.FileExists(cpath) Then
                My.Computer.FileSystem.WriteAllText(cpath,
                                                    curdate & Strings.Space(9) &
                                                    caseno.PadRight(15, " ") & Strings.Space(5) &
                                                    earnedTextBox.Text.PadLeft(5, " ") & Strings.Space(17) &
                                                    tlable.PadRight(8) & Strings.Space(6) &
                                                    takenTextBox.Text.PadLeft(5, " ") & Strings.Space(10) &
                                                    Convert.ToString(previous).PadLeft(5, " ") & ControlChars.NewLine, True)

                Separation()

                MessageBox.Show("Processing complete. The form will be cleared.",
                                title, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Me.Show()
                newbalLabel.Text = "0.00"
                calcearnedTextBox.Text = "Ready"
                caseComboBox.Text = ""
                earnedTextBox.Clear()
                takenTextBox.Clear()
                accruedRadioButton.Select()
                sctComboBox.SelectedItem = "Comp Time (X 1.5)"
                newbalance = 0D
                accruedDateTimePicker.Focus()
                Me.btnApply.Enabled = False

                'ability to detect 50 hours csr for warning
                If Me.prevbalLabel.Text >= 50.0 Then
                    Me.prevbalLabel.ForeColor = Color.Red
                    Me.warningLbl.Show()
                Else
                    Me.warningLbl.Hide()
                    Me.prevbalLabel.ForeColor = Color.Black

                End If

                'If Comptime file does not exists, the program creates it and 
                'writes current balance text file
            Else : My.Computer.FileSystem.CreateDirectory(cdirectory)
                My.Computer.FileSystem.WriteAllText(cpath,
                                                    "Orange County Juvenile Probation Dept" & ControlChars.NewLine &
                                                    "---------------------------------------" & ControlChars.NewLine &
                                                    "Personal Comptime Account for: " & user & ControlChars.NewLine & ControlChars.NewLine &
                                                    heading & ControlChars.NewLine &
                                                    "-------------" & Strings.Space(6) &
                                                    "----------" & Strings.Space(11) &
                                                    "------------" & Strings.Space(9) &
                                                    "----------" & Strings.Space(5) &
                                                    "----------" & Strings.Space(4) &
                                                    "----------" & ControlChars.NewLine, True)

                My.Computer.FileSystem.WriteAllText(cpath, curdate & Strings.Space(9) &
                                                    caseno.PadRight(15, " ") & Strings.Space(5) &
                                                    earnedTextBox.Text.PadLeft(5, " ") & Strings.Space(17) &
                                                    tlable.PadRight(8) & Strings.Space(6) &
                                                    takenTextBox.Text.PadLeft(5, " ") & Strings.Space(10) &
                                                    Convert.ToString(previous).PadLeft(5, " ") & ControlChars.NewLine, True)

                Separation()

                MessageBox.Show("Processing complete. The form will be cleared.",
                                title, MessageBoxButtons.OK, MessageBoxIcon.Information)

                Me.Show()
                newbalLabel.Text = "0.00"
                calcearnedTextBox.Text = "Ready"
                caseComboBox.Text = ""
                earnedTextBox.Clear()
                takenTextBox.Clear()
                accruedRadioButton.Select()
                sctComboBox.SelectedItem = "Comp Time (X 1.5)"
                newbalance = 0D
                accruedDateTimePicker.Focus()
                Me.btnApply.Enabled = False

                'ability to detect 50 hours csr for warning
                If Me.prevbalLabel.Text >= 50.0 Then
                    Me.prevbalLabel.ForeColor = Color.Red
                    Me.warningLbl.Show()
                Else
                    Me.warningLbl.Hide()
                    Me.prevbalLabel.ForeColor = Color.Black

                End If

            End If

            'If user does not want to make calculation, the program
            'will ask if the user wants to return to the program for another calculation.
            'If the user does not, then the program will direct user to exit.
        Else : my_apply = Windows.Forms.DialogResult.No
            my_another = MessageBox.Show("Do you want to make another calculation?", title,
        MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If my_another = Windows.Forms.DialogResult.Yes Then
                Me.Show()
                newbalLabel.Text = "0.00"
                calcearnedTextBox.Text = "Ready"
                caseComboBox.Text = ""
                earnedTextBox.Clear()
                takenTextBox.Clear()
                accruedRadioButton.Select()
                sctComboBox.SelectedItem = "Comp Time (X 1.5)"
                newbalance = 0D
                accruedDateTimePicker.Focus()
                Me.btnApply.Enabled = False

                'ability to detect 50 hours csr for warning
                If Me.prevbalLabel.Text >= 50.0 Then
                    Me.prevbalLabel.ForeColor = Color.Red
                    Me.warningLbl.Show()
                Else
                    Me.warningLbl.Hide()
                    Me.prevbalLabel.ForeColor = Color.Black

                End If

            Else : my_another = Windows.Forms.DialogResult.No
                MessageBox.Show("No calcuation will be made and the form will be reset. You may exit the program.", title,
                                MessageBoxButtons.OK, MessageBoxIcon.Information)
                Me.Show()
                newbalLabel.Text = "0.00"
                calcearnedTextBox.Text = "Ready"
                caseComboBox.Text = ""
                earnedTextBox.Clear()
                takenTextBox.Clear()
                sctComboBox.SelectedItem = "Comp Time (X 1.5)"
                accruedRadioButton.Select()
                newbalance = 0D
                accruedDateTimePicker.Focus()
                Me.btnApply.Enabled = False

                'ability to detect 50 hours csr for warning
                If Me.prevbalLabel.Text >= 50.0 Then
                    Me.prevbalLabel.ForeColor = Color.Red
                    Me.warningLbl.Show()
                Else
                    Me.warningLbl.Hide()
                    Me.prevbalLabel.ForeColor = Color.Black

                End If

            End If

        End If

    End Sub

    Public Sub CreateMyPaths()

        'Only used as a placeholder on first run if no prior transaction is completed

        'Declare text writing variables
        Dim line As String
        Dim caseno As String = "Placeholder"
        Dim curdate As String = accruedDateTimePicker.Text

        'make calculations
        newbalance = Convert.ToDecimal(newbalLabel.Text)
        newbalance = Math.Round(newbalance, 2)
        previous = Convert.ToDecimal(prevbalLabel.Text)
        previous += newbalance
        previous = Math.Round(previous, 2)
        prevbalLabel.Text = Convert.ToString(previous)

        'Convert the data from the Previous Balance Label and store it in line variable
        line = Convert.ToString(previous)

        MyLabels()

        ' If Comptime Directory exists, writes to the file.
        If My.Computer.FileSystem.DirectoryExists(cdirectory) Then

            My.Computer.FileSystem.WriteAllText(cpath,
                                            "Orange County Juvenile Probation Dept" & ControlChars.NewLine &
                                            "---------------------------------------" & ControlChars.NewLine &
                                            "Personal Comptime Account for: " & user & ControlChars.NewLine & ControlChars.NewLine &
                                            heading & ControlChars.NewLine &
                                            "-------------" & Strings.Space(6) &
                                            "----------" & Strings.Space(11) &
                                            "------------" & Strings.Space(9) &
                                            "----------" & Strings.Space(5) &
                                            "----------" & Strings.Space(4) &
                                            "----------" & ControlChars.NewLine, True)

            My.Computer.FileSystem.WriteAllText(cpath,
                                            curdate & Strings.Space(9) &
                                            caseno.PadRight(15, " ") & Strings.Space(5) &
                                            "0.00".PadLeft(5, " ") & Strings.Space(17) &
                                            tlable.PadRight(8) & Strings.Space(6) &
                                            "0.00".PadLeft(5, " ") & Strings.Space(10) &
                                            Convert.ToString(previous).PadLeft(5, " ") & ControlChars.NewLine, True)
            Separation()

        Else
            ' Comptime Directory Does not Exist. Creates Comptime Directory and Comptime Bank
            My.Computer.FileSystem.CreateDirectory(cdirectory)

            My.Computer.FileSystem.WriteAllText(cpath,
                                            "Orange County Juvenile Probation Dept" & ControlChars.NewLine &
                                            "---------------------------------------" & ControlChars.NewLine &
                                            "Personal Comptime Account for: " & user & ControlChars.NewLine & ControlChars.NewLine &
                                            heading & ControlChars.NewLine &
                                            "-------------" & Strings.Space(6) &
                                            "----------" & Strings.Space(11) &
                                            "------------" & Strings.Space(9) &
                                            "----------" & Strings.Space(5) &
                                            "----------" & Strings.Space(4) &
                                            "----------" & ControlChars.NewLine, True)

            My.Computer.FileSystem.WriteAllText(cpath,
                                            curdate & Strings.Space(9) &
                                            caseno.PadRight(15, " ") & Strings.Space(5) &
                                            "0.00".PadLeft(5, " ") & Strings.Space(17) &
                                            tlable.PadRight(8) & Strings.Space(6) &
                                            "0.00".PadLeft(5, " ") & Strings.Space(10) &
                                            Convert.ToString(previous).PadLeft(5, " ") & ControlChars.NewLine, True)
            Separation()
        End If

    End Sub

    Public Sub PreviewCalculations()

        'declare calculation variables
        Dim earned As Decimal
        Dim calcearned As Decimal
        Dim taken As Decimal
        Dim isEarned As Boolean
        Dim isTaken As Boolean
        Dim previewbankbal As Decimal

        'Determine if this time is accrued or taken
        If accruedRadioButton.Checked Then
            If takenTextBox.Text = String.Empty Then
                takenTextBox.Text = "0.00"
            End If

            MyLabels()

            'Convert Input
            isEarned = Decimal.TryParse(earnedTextBox.Text, earned)
            isTaken = Decimal.TryParse(takenTextBox.Text, taken)

            'If conversions successful, make calculations
            If isEarned And isTaken Then
                If atime = "Straight Time (X 1.0)" Or atime = "n/a" Then
                    calcearned = earned * 1D

                Else
                    calcearned = earned * 1.5D

                End If

                calcearned = Math.Round(calcearned, 2)
                previewbankbal = calcearned + Convert.ToDecimal(prevbalLabel.Text)
                calcearnedTextBox.Text = ""

                calcearnedTextBox.Text = "Total accrued time to enter on affidavit = " &
                                        (calcearned).ToString("N2") & " hours" & ControlChars.NewLine &
                                        "=".PadLeft(80, "=") & ControlChars.NewLine &
                                        "Preview of Entry to Activity Sheet:" & ControlChars.NewLine & ControlChars.NewLine &
                                        "Date Entered" & Strings.Space(14) &
                                        "CaseNo." & Strings.Space(14) &
                                        "Earned(+)" & Strings.Space(12) &
                                        "Type" & Strings.Space(22) &
                                        "Taken(-)" & Strings.Space(16) &
                                        "Balance" & ControlChars.NewLine &
                                        "-----------------" & Strings.Space(13) &
                                        "----------" & Strings.Space(16) &
                                        "------------" & Strings.Space(13) &
                                        "----------" & Strings.Space(17) &
                                        "----------" & Strings.Space(17) &
                                        "----------" & ControlChars.NewLine &
                                        accruedDateTimePicker.Text & Strings.Space(16) &
                                        caseComboBox.Text.PadRight(15, " ") & Strings.Space(7) &
                                        earnedTextBox.Text.PadLeft(5, " ") & Strings.Space(22) &
                                        tlable.PadRight(8) & Strings.Space(16) &
                                        takenTextBox.Text.PadLeft(5, " ") & Strings.Space(22) &
                                        Convert.ToString(previewbankbal).PadLeft(5, " ")

                newbalance = calcearned - taken
                newbalance = Math.Round(newbalance, 2)
                newbalLabel.Text = Convert.ToString(newbalance)

            Else : MessageBox.Show("Must be numeric", title, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information)
                earnedTextBox.Focus()
            End If

        ElseIf spentRadioButton.Checked Then

            If earnedTextBox.Text = String.Empty Then
                earnedTextBox.Text = "0.00"
            End If

            MyLabels()

            'Convert input
            isEarned = Decimal.TryParse(earnedTextBox.Text, earned)
            isTaken = Decimal.TryParse(takenTextBox.Text, taken)

            'If conversions successful, make calculations
            If isEarned And isTaken Then
                If atime = "Straight Time (X 1.0)" Or atime = "n/a" Then
                    calcearned = earned * 1D

                Else
                    calcearned = earned * 1.5D

                End If

                calcearned = Math.Round(calcearned, 2)
                newbalance = calcearned - taken
                newbalance = Math.Round(newbalance, 2)
                previewbankbal = newbalance + Convert.ToDecimal(prevbalLabel.Text)
                newbalLabel.Text = Convert.ToString(newbalance)
                calcearnedTextBox.Text = ""

                calcearnedTextBox.Text = "Total taken time to enter on affidavit = " &
                                        (taken).ToString("N2") &
                                        " hours" & ControlChars.NewLine & "-".PadLeft(80, "=") & ControlChars.NewLine &
                                        "Preview of Entry to Activity Sheet:" & ControlChars.NewLine & ControlChars.NewLine &
                                        "Date Entered" & Strings.Space(14) &
                                        "CaseNo." & Strings.Space(14) &
                                        "Earned(+)" & Strings.Space(12) &
                                        "Type" & Strings.Space(22) &
                                        "Taken(-)" & Strings.Space(16) &
                                        "Balance" & ControlChars.NewLine &
                                        "-----------------" & Strings.Space(13) &
                                        "----------" & Strings.Space(16) &
                                        "------------" & Strings.Space(13) &
                                        "----------" & Strings.Space(17) &
                                        "----------" & Strings.Space(17) &
                                        "----------" & ControlChars.NewLine &
                                        accruedDateTimePicker.Text & Strings.Space(16) &
                                        caseComboBox.Text.PadRight(15, " ") & Strings.Space(7) &
                                        earnedTextBox.Text.PadLeft(5, " ") & Strings.Space(22) &
                                        tlable.PadRight(8) & Strings.Space(16) &
                                        takenTextBox.Text.PadLeft(5, " ") & Strings.Space(22) &
                                        Convert.ToString(previewbankbal)

            Else : MessageBox.Show("Must be numeric", title, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information)
                takenTextBox.Focus()

            End If

        End If

        Me.btnApply.Enabled = True
        Me.ApplyToolStripMenuItem.Enabled = True

    End Sub

    Public Sub CalcClear()

        'clears the form 

        'declares variables
        Dim my_choice As DialogResult

        my_choice = MessageBox.Show("Do you wish to add to new balance to the bank?",
                                    title, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If my_choice = Windows.Forms.DialogResult.Yes Then
            'declare block variables
            Dim curdate As String
            Dim caseno As String
            Dim line2 As String

            'make calculations
            newbalance = Convert.ToDecimal(newbalLabel.Text)
            newbalance = Math.Round(newbalance, 2)
            previous = Convert.ToDecimal(prevbalLabel.Text)
            previous += newbalance
            previous = Math.Round(previous, 2)
            prevbalLabel.Text = Convert.ToString(previous)

            'convert data
            caseno = caseComboBox.Text
            curdate = accruedDateTimePicker.Text
            line2 = Convert.ToString(previous)

            MyLabels()

            'Write current balance text file
            If my_choice = Windows.Forms.DialogResult.Yes And My.Computer.FileSystem.FileExists(cpath) Then
                My.Computer.FileSystem.WriteAllText(cpath,
                                                    curdate & Strings.Space(9) &
                                                    caseno.PadRight(15, " ") & Strings.Space(5) &
                                                    earnedTextBox.Text.PadLeft(5, " ") & Strings.Space(17) &
                                                    tlable.PadRight(8) & Strings.Space(6) &
                                                    takenTextBox.Text.PadLeft(5, " ") & Strings.Space(10) &
                                                    Convert.ToString(previous) & ControlChars.NewLine, True)
                Separation()

            Else 'Setting up for the first time
                My.Computer.FileSystem.CreateDirectory(cdirectory)
                My.Computer.FileSystem.WriteAllText(cpath,
                                                    heading & ControlChars.NewLine &
                                                    "-------------" & Strings.Space(6) &
                                                    "----------" & Strings.Space(11) &
                                                    "------------" & Strings.Space(9) &
                                                    "----------" & Strings.Space(5) &
                                                    "----------" & Strings.Space(4) &
                                                    "----------" & ControlChars.NewLine, True)

                My.Computer.FileSystem.WriteAllText(cpath,
                                                    curdate & Strings.Space(9) &
                                                    caseno.PadRight(15, " ") & Strings.Space(5) &
                                                    earnedTextBox.Text.PadLeft(5, " ") & Strings.Space(17) &
                                                    tlable.PadRight(8) & Strings.Space(6) &
                                                    takenTextBox.Text.PadLeft(5, " ") & Strings.Space(10) &
                                                    Convert.ToString(previous) & ControlChars.NewLine, True)
                Separation()
            End If

            CleanHouse()

            MyWarning()

        Else : my_choice = Windows.Forms.DialogResult.No

            Me.Show()
            newbalance = 0D

            CleanHouse()

            MyWarning()

        End If
    End Sub

    Private Sub exitApp()
        'Exits the Program

        Dim my_result As DialogResult

        my_result = MessageBox.Show("Are you sure that you are ready to exit?", title,
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If my_result = Windows.Forms.DialogResult.No Then

            Me.Show()

            CleanHouse()

            MyWarning()

        Else : my_result = Windows.Forms.DialogResult.Yes
            If My.Computer.FileSystem.FileExists(cpath) Then

                Me.Close()

            Else : CreateMyPaths()

                Me.Close()

            End If

        End If
    End Sub

    Private Sub Separation()

        My.Computer.FileSystem.WriteAllText(cpath, "".PadLeft(100, "-") & ControlChars.NewLine, True)

    End Sub

    Private Sub MyLabels()

        'If statement to determine straight, comptime, or n/a.
        atime = Me.sctComboBox.Text

        If atime.Contains("Straight") Then
            tlable = "Straight"
        ElseIf atime.Contains("Comp") Then
            tlable = "Comp"
        Else
            tlable = "n/a"

        End If

    End Sub

    Private Sub MyWarning()

        'ability to detect 50 hours csr for warning
        If Me.prevbalLabel.Text >= 50.0 Then
            Me.prevbalLabel.ForeColor = Color.Red
            Me.warningLbl.Show()
        Else
            Me.warningLbl.Hide()
            Me.prevbalLabel.ForeColor = Color.Black

        End If

    End Sub

    Private Sub CleanHouse()

        newbalLabel.Text = "0.00"
        calcearnedTextBox.Text = ""
        calcearnedTextBox.Text = "Ready"
        caseComboBox.SelectedItem = "[Enter One]"
        sctComboBox.SelectedItem = "Comp Time (X 1.5)"
        earnedTextBox.Clear()
        takenTextBox.Clear()
        accruedRadioButton.Select()
        accruedDateTimePicker.Focus()
        Me.btnApply.Enabled = False

    End Sub

    '------------------------------ Buttons and Click Events ---------------------------------------------------

    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        exitApp()

    End Sub

    Private Sub btnReconcile_Click(sender As Object, e As EventArgs) Handles btnReconcile.Click
        Me.Hide()
        My.Forms.frm_Reconcile.Show()

    End Sub

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        CalcClear()

    End Sub

    Private Sub btnCalc_Click(sender As Object, e As EventArgs) Handles btnCalc.Click
        PreviewCalculations()

    End Sub

    Private Sub btnApply_Click(sender As Object, e As EventArgs) Handles btnApply.Click
        ApplyCalculations()

    End Sub

End Class
