namespace Model

open System

type User = {Id: Document; Email: Email}
with
    static member Create documentString emailString =
        {Id = Document.FromString documentString; Email = Email.Create emailString}