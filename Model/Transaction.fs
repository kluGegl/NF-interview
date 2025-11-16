namespace Model

open System

//Payee and TimeStamp will be payment identifier
type Payment = {Payee: User; Amount: Decimal; TimeStamp: DateTime}
with
    member x.Cents = Decimal.Multiply (x.Amount, 2M) 

type Reversal = {Transaction: Transaction; TimeStamp: DateTime}

and Transaction =
    | Payment of Payment
    | Reversal of Reversal
with
    member x.GetPayee =
        match x with
        | Payment payment -> payment.Payee
        | Reversal reversal -> reversal.Transaction.GetPayee

    member x.GetAmount =
        match x with
        | Payment payment -> payment.Amount
        | Reversal reversal -> reversal.Transaction.GetAmount