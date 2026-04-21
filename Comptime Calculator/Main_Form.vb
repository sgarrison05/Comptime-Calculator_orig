'Title                  Comptime Calculator (Original Ver)
'Purpose                To calculate comptime time earned or spent
'                       in a particular instance
'Created By             Shon Garrison, December 2008
'Updated Last           April 2026

'Update Notes:          Added the ability to use straight or comp time.
'                       Before reformat
Option Explicit On

Public Class frm_Main

    Private Const CDIRECTORY As String = "D:\Temp\Comptime"
    Private Const CPATH As String = "D:\Temp\Comptime\comptimerun.txt"
    Private Const TITLE As String = "Comptime Calculator"
    Private Const WARNING_HOURS As Decimal = 50D
    Private Const STRAIGHT_TIME_MULTIPLIER As Decimal = 1D
    Private Const COMP_TIME_MULTIPLIER As Decimal = 1.5D

    Public userName As String
    Private _newBalance As Decimal
    Private _previousBalance As Decimal
    Private _lastEntryBalance As String
    Private _timeTypeLabel As String
    Private _selectedTimeType As String

    Private ReadOnly _heading As String =
        "Date Entered" & Strings.Space(7) &
        "CaseNo." & Strings.Space(14) &
        "Earned(+)" & Strings.Space(12) &
        "Type" & Strings.Space(11) &
        "Taken(-)" & Strings.Space(6) &
        "Balance"

    Private ReadOnly _columnDivider As String =
        "-------------" & Strings.Space(6) &
        "----------" & Strings.Space(11) &
        "------------" & Strings.Space(9) &
        "----------" & Strings.Space(5) &
        "----------" & Strings.Space(4) &
        "----------"

    Private Sub compcalcForm_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        InitializeControls()

        If My.Computer.FileSystem.FileExists(CPATH) Then
            LoadExistingBankFile()
        Else
            PromptToCreateBankFile()
        End If

    End Sub

    '------------------------------- Private Functions and Subroutines -----------------------------------------
    Public Sub InitializeControls()

        Me.calcearnedTextBox.ReadOnly = True
        Me.btnApply.Enabled = False
        Me.ApplyToolStripMenuItem.Enabled = False

        ' Case/Reason combo box
        With Me.caseComboBox.Items
            .Add("[Enter One]")
            .Add("Sick")
            .Add("Personal")
            .Add("Dr. Appt")
            .Add("New Case")
            .Add("Transport")
            .Add("Det Visit")
            .Add("On-Call")
            .Add("Spec Group")
            .Add("Plcmt Visit")
            .Add("Training")
            .Add("Evaluation")
            .Add("Meeting")
        End With
        Me.caseComboBox.SelectedItem = "[Enter One]"

        ' Straight/Comp time combo box
        With Me.sctComboBox.Items
            .Add("Straight Time (X 1.0)")
            .Add("Comp Time (X 1.5)")
            .Add("n/a")
        End With
        Me.sctComboBox.SelectedItem = "Comp Time (X 1.5)"

        Me.accruedRadioButton.Select()
        Me.accruedDateTimePicker.Focus()

    End Sub

    Private Sub LoadExistingBankFile()

        Try
            Dim fileText As String = My.Computer.FileSystem.ReadAllText(CPATH)
            Dim entryIndex As Integer = 0
            Dim newLineIndex As Integer = fileText.IndexOf(ControlChars.NewLine, entryIndex)

            Do Until newLineIndex = -1
                Dim entry As String = fileText.Substring(entryIndex, newLineIndex - entryIndex)

                ' Extract the running balance from any line that contains a date
                If entry.Contains("/") Then
                    _lastEntryBalance = Trim(Microsoft.VisualBasic.Right(entry, 7))
                End If

                ' Extract the username from the account header line
                If entry.Contains("Account") Then
                    userName = entry.Substring(31)
                End If

                entryIndex = newLineIndex + 2
                newLineIndex = fileText.IndexOf(ControlChars.NewLine, entryIndex)
            Loop

            Me.prevbalLabel.Text = If(_lastEntryBalance, "0.00")
            Me.Text = "Personal Comptime Calculator for " & userName
            Me.newbalLabel.Text = "0.00"
            Me.calcearnedTextBox.Text = "Ready"

            UpdateWarningDisplay()

        Catch ex As Exception
            MessageBox.Show("Error reading comptime file:" & Environment.NewLine & ex.Message,
                            TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub PromptToCreateBankFile()

        Dim createResult As DialogResult =
            MessageBox.Show("The current comptime balance file does not exist. " &
                            "This is your comptime bank — would you like to create it?",
                            TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If createResult = DialogResult.No Then
            Me.Close()
            Return
        End If

        Dim hasBalance As DialogResult =
            MessageBox.Show("Do you have a current balance to enter?",
                            TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If hasBalance = DialogResult.Yes Then

            ' Keep prompting until a valid numeric balance is entered
            Dim balanceInput As String = String.Empty
            Do
                balanceInput = InputBox("Please enter current balance or click 'Ok' to start at zero.",
                                        TITLE, "0.00")
                If Not IsNumeric(balanceInput) Then
                    MessageBox.Show("Balance must be a number.", TITLE, MessageBoxButtons.OK)
                End If
            Loop Until IsNumeric(balanceInput)

            userName = InputBox("Please enter your name.", TITLE)
            Me.prevbalLabel.Text = balanceInput

        Else
            Me.prevbalLabel.Text = "0.00"
        End If

        Me.Text = "Personal Comptime Calculator for " & userName
        Me.newbalLabel.Text = "0.00"
        Me.calcearnedTextBox.Text = "Ready"
        UpdateWarningDisplay()

    End Sub

    Private Sub UpdateWarningDisplay()

        Dim currentBalance As Decimal
        If Decimal.TryParse(Me.prevbalLabel.Text, currentBalance) AndAlso
           currentBalance >= WARNING_HOURS Then
            Me.prevbalLabel.ForeColor = Color.Red
            Me.warningLbl.Show()
        Else
            Me.prevbalLabel.ForeColor = Color.Black
            Me.warningLbl.Hide()
        End If

    End Sub

    Public Sub ApplyCalculations()
        'Saves current balance to txt file

        Dim applyResult As DialogResult =
                    MessageBox.Show("Do you wish to add the new balance to the bank?",
                                    TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If applyResult = DialogResult.Yes Then

            CommitBalanceUpdate()
            WriteTransactionToFile()

            MessageBox.Show("Processing complete. The form will be cleared.",
                            TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            Dim continueResult As DialogResult =
                MessageBox.Show("Do you want to make another calculation?",
                                TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question)

            If continueResult = DialogResult.No Then
                MessageBox.Show("No calculation will be made and the form will be reset. " &
                                "You may exit the program.",
                                TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End If

        ' Reset the form regardless of whether the user saved or not
        CleanHouse()
        UpdateWarningDisplay()

    End Sub

    Public Sub CreatePlaceholderEntry()

        'Only used as a placeholder on first run if no prior transaction is completed
        Try
            ResolveTimeTypeLabel()

            _newBalance = Math.Round(Convert.ToDecimal(Me.newbalLabel.Text), 2)
            _previousBalance = Math.Round(Convert.ToDecimal(Me.prevbalLabel.Text) + _newBalance, 2)
            Me.prevbalLabel.Text = Convert.ToString(_previousBalance)

            If Not My.Computer.FileSystem.FileExists(CPATH) Then
                WriteFileHeader()
            End If

            Dim placeholderRow As String =
                Me.accruedDateTimePicker.Text & Strings.Space(9) &
                "Placeholder".PadRight(15, " "c) & Strings.Space(5) &
                "0.00".PadLeft(5, " "c) & Strings.Space(17) &
                _timeTypeLabel.PadRight(8) & Strings.Space(6) &
                "0.00".PadLeft(5, " "c) & Strings.Space(10) &
                Convert.ToString(_previousBalance).PadLeft(5, " "c) & ControlChars.NewLine

            My.Computer.FileSystem.WriteAllText(CPATH, placeholderRow, True)
            Separation()

        Catch ex As Exception
            MessageBox.Show("Error creating placeholder entry:" & Environment.NewLine & ex.Message,
                            TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try


    End Sub

    Public Sub PreviewCalculations()

        'declare calculation variables
        Dim earned As Decimal
        Dim taken As Decimal
        Dim calcEarned As Decimal
        Dim previewBalance As Decimal

        ResolveTimeTypeLabel()

        'Determine if this time is accrued or taken
        If accruedRadioButton.Checked Then
            If takenTextBox.Text = String.Empty Then takenTextBox.Text = "0.00"

            If Not Decimal.TryParse(earnedTextBox.Text, earned) OrElse
            Not Decimal.TryParse(takenTextBox.Text, taken) Then
                MessageBox.Show("Hours entered must be numeric.", TITLE,
                                MessageBoxButtons.OK, MessageBoxIcon.Information)
                Me.earnedTextBox.Focus()
                Return
            End If

            'If successful, make calculations
            calcEarned = Math.Round(calcEarned * GetTimeMultiplier(), 2)
            previewBalance = calcEarned + Convert.ToDecimal(prevbalLabel.Text)
            _newBalance = Math.Round(calcEarned - taken, 2)
            Me.newbalLabel.Text = Convert.ToString(_newBalance)

            Me.calcearnedTextBox.Text = "Total accrued time to enter on affidavit = " &
                calcEarned.ToString("N2") &
                " hours" &
                BuildPreviewText(previewBalance)

        ElseIf spentRadioButton.Checked Then

            If earnedTextBox.Text = String.Empty Then earnedTextBox.Text = "0.00"

            If Not Decimal.TryParse(Me.earnedTextBox.Text, earned) OrElse
               Not Decimal.TryParse(Me.takenTextBox.Text, taken) Then
                MessageBox.Show("Hours entered must be numeric.", TITLE,
                                MessageBoxButtons.OK, MessageBoxIcon.Information)
                Me.takenTextBox.Focus()
                Return
            End If

            calcEarned = Math.Round(calcEarned * GetTimeMultiplier(), 2)
            _newBalance = Math.Round(_newBalance, 2)
            previewBalance = _newBalance + Convert.ToDecimal(prevbalLabel.Text)
            newbalLabel.Text = Convert.ToString(_newBalance)
            calcearnedTextBox.Text = ""

            calcearnedTextBox.Text = "Total taken time to enter on affidavit = " &
                                        taken.ToString("N2") &
                                        " hours" &
                                        BuildPreviewText(previewBalance)
        End If

        Me.btnApply.Enabled = True
        Me.ApplyToolStripMenuItem.Enabled = True

    End Sub

    Public Sub CalcClear()

        'clears the form 
        Dim saveResult As DialogResult =
            MessageBox.Show("Do you wish to add the new balance to the bank?",
                            TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If saveResult = DialogResult.Yes Then
            CommitBalanceUpdate()
            WriteTransactionToFile()
        End If

        CleanHouse()
        UpdateWarningDisplay()

    End Sub

    Private Sub exitApp()
        'Exits the Program

        Dim exitResult As DialogResult
        exitResult = MessageBox.Show("Are you sure that you are ready to exit?", TITLE,
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If exitResult = Windows.Forms.DialogResult.No Then
            CleanHouse()
            UpdateWarningDisplay()
            Return
        End If

        If Not My.Computer.FileSystem.FileExists(CPATH) Then
            CreatePlaceholderEntry()
        End If

        Me.Close()

    End Sub

    Private Sub Separation()

        My.Computer.FileSystem.WriteAllText(cpath, "".PadLeft(100, "-") & ControlChars.NewLine, True)

    End Sub

    Private Sub ResolveTimeTypeLabel()

        'If statement to determine straight, comptime, or n/a.
        _selectedTimeType = Me.sctComboBox.Text

        If _selectedTimeType.Contains("Straight") Then
            _timeTypeLabel = "Straight"
        ElseIf _selectedTimeType.Contains("Comp") Then
            _timeTypeLabel = "Comp"
        Else
            _timeTypeLabel = "n/a"

        End If

    End Sub

    Private Sub CleanHouse()

        newbalLabel.Text = "0.00"
        calcearnedTextBox.Text = "Ready"
        caseComboBox.SelectedItem = "[Enter One]"
        sctComboBox.SelectedItem = "Comp Time (X 1.5)"
        earnedTextBox.Clear()
        takenTextBox.Clear()
        accruedRadioButton.Select()
        accruedDateTimePicker.Focus()
        Me.btnApply.Enabled = False
        Me.ApplyToolStripMenuItem.Enabled = False
        _newBalance = 0D

    End Sub

    Private Sub CommitBalanceUpdate()

        _newBalance = Math.Round(Convert.ToDecimal(Me.newbalLabel.Text), 2)
        _previousBalance = Math.Round(Convert.ToDecimal(Me.prevbalLabel.Text) + _newBalance, 2)
        Me.prevbalLabel.Text = Convert.ToString(_previousBalance)

    End Sub

    Private Sub WriteTransactionToFile()

        Try
            Dim currentDate As String = Me.accruedDateTimePicker.Text
            Dim caseNo As String = Me.caseComboBox.Text
            Dim earnedText As String = Me.earnedTextBox.Text
            Dim takenText As String = Me.takenTextBox.Text
            Dim balanceText As String = Convert.ToString(_previousBalance)

            If Not My.Computer.FileSystem.FileExists(CPATH) Then
                WriteFileHeader()
            End If

            Dim transactionRow As String =
                currentDate & Strings.Space(9) &
                caseNo.PadRight(15, " "c) & Strings.Space(5) &
                earnedText.PadLeft(5, " "c) & Strings.Space(17) &
                _timeTypeLabel.PadRight(8) & Strings.Space(6) &
                takenText.PadLeft(5, " "c) & Strings.Space(10) &
                balanceText.PadLeft(5, " "c) & ControlChars.NewLine

            My.Computer.FileSystem.WriteAllText(CPATH, transactionRow, True)
            Separation()

        Catch ex As Exception
            MessageBox.Show("Error writing to comptime file:" & Environment.NewLine & ex.Message,
                            TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub WriteFileHeader()

        If Not My.Computer.FileSystem.DirectoryExists(CDIRECTORY) Then
            My.Computer.FileSystem.CreateDirectory(CDIRECTORY)
        End If

        My.Computer.FileSystem.WriteAllText(CPATH,
            "Orange County Juvenile Probation Dept" & ControlChars.NewLine &
            "---------------------------------------" & ControlChars.NewLine &
            "Personal Comptime Account for: " & userName & ControlChars.NewLine &
            ControlChars.NewLine &
            _heading & ControlChars.NewLine &
            _columnDivider & ControlChars.NewLine, True)

    End Sub

    Private Function GetTimeMultiplier() As Decimal
        If _selectedTimeType.Contains("Straight") OrElse _selectedTimeType = "n/a" Then
            Return STRAIGHT_TIME_MULTIPLIER
        Else
            Return COMP_TIME_MULTIPLIER
        End If
    End Function

    Private Function BuildPreviewText(previewBalance As Decimal) As String
        Return ControlChars.NewLine &
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
               _timeTypeLabel.PadRight(8) & Strings.Space(16) &
               takenTextBox.Text.PadLeft(5, " ") & Strings.Space(22) &
               Convert.ToString(previewBalance).PadLeft(5, " ")
    End Function
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
