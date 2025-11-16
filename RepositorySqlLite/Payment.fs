module RepositorySqlLite.Payment

open System
open Impl
open Model

type Decimal
with
    static member FromCents cents = Decimal.Divide(cents, 2M) 
    member x.Cents = Decimal.Multiply(x, 100M)

let private oneMillisecondTicks = TimeSpan.FromMilliseconds(1L).Ticks

let private insertPaymentQuery = """INSERT INTO Payment (payeeId, amountCents, "timeStamp")
    VALUES ($payee, $amount, $timeStamp);"""

let private insertReversePaymentQuery = """INSERT INTO Reversal (paymentPayeeId, paymentTimeStamp, timeStamp)
    VALUES ($paymentPayeeId, $paymentTimeStamp, $timeStamp)"""

let private balanceForUserQuery = """SELECT SUM(amountCents) FROM Payment
    LEFT JOIN Reversal ON Reversal.paymentPayeeId = Payment.payeeId
        AND Reversal.paymentTimeStamp = Payment.timeStamp
    WHERE payeeId = $payeeId AND Reversal.paymentPayeeId IS NULL"""

let insertPayment (payeeId: Document) (amount: Decimal) (timeStamp: DateTime) connection =
    match executeNonQuery connection insertPaymentQuery
        [("payee", payeeId.Id)
         ("amount", amount.Cents)
         ("timeStamp", box timeStamp.Ticks)] with
    | 1 -> 1
    | _ -> failwith $"Unable to insert payment {payeeId}; {amount}; {timeStamp}"

let insertReversal (paymentPayeeId: Document) (paymentTimeStamp: DateTime) (timeStamp: DateTime) connection =
    match executeNonQuery connection insertReversePaymentQuery
        [
            ("$paymentPayeeId", paymentPayeeId.Id)
            ("$paymentTimeStamp", paymentTimeStamp.Ticks)
            ("$timeStamp", timeStamp.Ticks)
        ] with
    | 1 -> 1
    | _ -> failwith $"Unable to reverse payment {paymentPayeeId} {paymentTimeStamp}"

let getBalanceFor (userId: Document) connection =
    executeScalar<int64> connection balanceForUserQuery [("$payeeId", userId.Id)]
    