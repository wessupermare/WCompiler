﻿Imports Runtime.Shared

Public Module Util
    Public Sub [Exit](ParamArray ReturnCode())
        Environment.Exit(If(ReturnCode.Length = 0, 0, ReturnCode(0)))
    End Sub

    ''' <see cref="Collect"/>
    Public Function C(ParamArray Data())
        Return Collect(Data)
    End Function

    Public Function Collect(ParamArray Data())
        If Data Is Nothing Then Return New List(Of Object)
        Return Data.ToList()
    End Function

    Public Function Dedup(ParamArray Data())
        Return C(Data.ToHashSet())
    End Function

    Public Function Split(ParamArray Data())
        Dim Content = Data(0)
        Data = Data.Skip(1).Select(Function(s) FormatArray(s)).ToArray()

        If Data.Length = 0 Then Data = {" "c, vbTab, vbCrLf, vbLf}

        If TypeOf Content Is IEnumerable(Of Object) AndAlso TypeOf Content IsNot String Then
            Dim retval As New List(Of String)
            For Each item In Content
                retval.AddRange(item.ToString().Split(Data.Select(Function(o) o.ToString()).ToArray(), StringSplitOptions.RemoveEmptyEntries))
            Next
            Return retval
        Else
            Return Content.ToString().Split(Data.Select(Function(o) o.ToString()).ToArray(), StringSplitOptions.RemoveEmptyEntries)
        End If
    End Function

    Public Function Thread(ParamArray Data())
        Dim Separator = Data(0)
        Data = Data.Skip(1).ToArray()
        If (Data.Length = 1) AndAlso (TypeOf Data(0) Is IEnumerable(Of Object) OrElse TypeOf Data(0) Is String) Then
            Return ThreadLocal(Separator, If(TypeOf Data(0) Is String, Data(0).ToString().ToCharArray().Select(Function(c) CObj(c)).ToArray(), Data(0)))
        Else
            Return Data.Aggregate(Function(threader, item) threader & Separator & If(TypeOf item Is IEnumerable(Of Object) AndAlso TypeOf item IsNot String, ThreadLocal(Separator, item), item))
        End If
    End Function

    Private Function ThreadLocal(Separator As Object, Data As IEnumerable(Of Object))
        Return Data.Aggregate(Function(threader, item) threader & Separator & If(TypeOf item Is IEnumerable(Of Object) AndAlso TypeOf item IsNot String, ThreadLocal(Separator, item), Unescape(item)))
    End Function
End Module