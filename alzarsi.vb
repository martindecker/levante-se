Imports System

Module Program
    Sub Main(args As String())
    For i As Integer = 0 To 4
                        Console.WriteLine(i)
                    Next
    For i As Integer = 1 to 9 step 2
                        Console.WriteLine(i)
                    Next
        Console.WriteLine("I began the project on August 22, 2018. Please enter sth to end the program.")
        Console.ReadKey()
    End Sub
End Module
