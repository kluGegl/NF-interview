namespace Model

open System
open System.Text.RegularExpressions

type Email = private Email of String
with
    static member Create (emailString: String) =
        if Regex.IsMatch (emailString, """^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$""")
        then Email emailString
        else failwithf $"{emailString} is not a valid email"