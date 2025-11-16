module Document

open System
open Xunit

open Model

[<Literal>]
let ValidPassportDocumentString = "PAS:1"
let ValidDrivingLicenseDocumentString = "DL:1"

[<Fact>]
let ``Create document from valid passport string`` () =
    let document = Document.FromString ValidPassportDocumentString
    Assert.Equal(DocumentType.InternationalPassport, document.DocumentType)
    Assert.Equal("1", document.DocumentId)

[<Fact>]
let ``Create document from valid driving license string`` () =
    let document = Document.FromString ValidDrivingLicenseDocumentString
    Assert.Equal(DocumentType.DrivingLicense, document.DocumentType)
    Assert.Equal("1", document.DocumentId)
    
[<Fact>]
let ``Fail to create from invalid document type`` () =
    let expectedException = Assert.Throws<Exception>(fun () -> Document.FromString "INV:1" |> ignore)
    Assert.Equal ("INV:1 is not valid document string", expectedException.Message)
    
[<Fact>]
let ``Fail to create without id`` () =
    let expectedException = Assert.Throws<Exception>(fun () -> Document.FromString "PAS:" |> ignore)
    Assert.Equal ("PAS: is not valid document string", expectedException.Message)