Imports System


Module Program
    Function janein1(text As String)
        Dim line As String
        Do
            Console.Write("{0} ? (j/n) ", text)
            line = Console.ReadLine()
        Loop Until "jnJN".Contains(line)
        Return line.ToUpper() = "J"
    End Function

    Public todo3 = VbLf

    Sub Main(args As String())
        Dim stoerungen2 = If( False, False, janein1("Straße arg befahren"))
        If stoerungen2 AndAlso DateTime.Today.DayOfWeek = DayOfWeek.Saturday Then
            todo3 = todo3 & "Ursache fuer stark befahrene Straße herausfinden"
        Else
            Console.WriteLine("nix")
        End If
        Console.WriteLine(todo3)
        Console.WriteLine("I began the project on August 22, 2018. Please enter sth to end the program.")
        Console.ReadKey()
    End Sub
End Module
