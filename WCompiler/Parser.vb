﻿Partial Module Parser
	Public References As String

	Private Property Lexer As Lexer
	Private outputbuffer As List(Of String)
	Private Filename As String

	Function Parse(Lexer As Lexer, filename As String) As String()
		outputbuffer = New List(Of String)
		Parser.Lexer = Lexer
		Parser.Filename = filename
		Program()
		Return outputbuffer.ToArray
	End Function

	Private Sub Program()
		Setup()
		Block()
		Teardown()
	End Sub

	Private Sub Block(Optional InLoop As Boolean = False, Optional InCond As Boolean = False)
		If InLoop OrElse InCond Then IndentLevel += 1
		Do Until Lexer.Current.Type = TokenType.EOF OrElse ((InLoop OrElse InCond) AndAlso Lexer.Current.Type = TokenType.RightSquare)
			Select Case Lexer.Current.Type
				Case TokenType.Escape
					Break()

				Case TokenType.If
					[If]()

				Case TokenType.Repeat
					[Loop]()

				Case TokenType.Item
					Declaration()

				Case TokenType.Variable
					Try
						Assignment()
					Catch ex As MissingFieldException
						[Function]()
					End Try

				Case TokenType.Link, TokenType.Ref
					Import()
			End Select
		Loop
		If InLoop OrElse InCond Then IndentLevel -= 1
	End Sub

	Private Sub [If]()
		Match(TokenType.If)
		BooleanExpr()
		Match(TokenType.LeftSquare)
		Emit("If Register Then")
		Block(InCond:=True)
		Match(TokenType.RightSquare)
		Emit("End If")
	End Sub

	Private Sub [Loop]()
		Match(TokenType.Repeat)
		Dim inf As Boolean = True
		If Lexer.Current.Type <> TokenType.LeftSquare Then
			Expr()
			inf = False
		End If
		Match(TokenType.LeftSquare)
		Emit("Stack.Push(Register)")
		Emit("Stack.Push(LoopEnd) : LoopEnd = Register")
		Emit("Stack.Push(Counter) : Counter = 1")
		Emit(If(inf, "Do", $"Do While Counter <= LoopEnd"))
		Block(InLoop:=True)
		Match(TokenType.RightSquare)
		Emit(vbTab & "Counter += 1")
		Emit("Loop")
		Emit("Counter = Stack.Pop()")
		Emit("LoopEnd = Stack.Pop()")
		Emit("Register = Stack.Pop()")
	End Sub

	Private Sub Break()
		Match(TokenType.Escape)
		Emit("Exit Do")
	End Sub

	Private ReadOnly Varlist As New List(Of String)
	Private Sub Declaration()
		Match(TokenType.Item)
		Dim varname = Match(TokenType.Variable).ToLower()
		If Lexer.Current.Type = TokenType.Equals Then
			Match(TokenType.Equals)
			Expr()
			Emit($"Variable(""{varname}"") = Register")
		Else
			Emit($"Variable(""{varname}"") = Nothing")
		End If
		Varlist.Add(varname)
	End Sub

	Private Sub Assignment()
		Dim varname = Match(TokenType.Variable, False).ToLower()
		If Not Varlist.Contains(varname) Then Throw New MissingFieldException("No variable named " & varname)
		Lexer.Advance()
		Match(TokenType.Equals)
		Expr()
		Emit($"Variable(""{varname}"") = Register")
	End Sub

	Private Sub Import()
		Select Case Lexer.Current.Type
			Case TokenType.Link
				Match(TokenType.Link)
				Throw New NotImplementedException("Statically linked libraries are currently unsupported.")
			Case TokenType.Ref
				Match(TokenType.Ref)
				AddLib(Match(TokenType.StringLiteral))
		End Select
	End Sub

	Private Sub [Function]()
		Dim funcName = Match(TokenType.Variable)

	End Sub
End Module