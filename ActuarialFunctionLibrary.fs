// Learn more about F# at http://fsharp.net
namespace ActuarialFunctionLibrary
    module AgeVector =

        // **************************************************************
        // Defines an AgeVector type which "generates" values at each age 
        // according to a generator function.
        // **************************************************************
        type Age = int
        type Term = int

        let (|ValidAge|InvalidAge|) (age : Age) =
            if age >= 0 && age <= 120 then
                ValidAge (age)
            else
                InvalidAge

        type boundaryBehaviour<'T> =
            | Zero of 'T
            | One of 'T
            | Fixed of 'T
            | Extend
            | Fail

        type IAgeVector<'T> =
            abstract member StartAge : Age
            abstract member EndAge : Age
            abstract member ValueAtAge : Age -> 'T
            abstract member LowerBoundBehaviour : boundaryBehaviour<'T>
            abstract member UpperBoundBehaviour : boundaryBehaviour<'T>

        type AgeVector<'T> (startAge,
                            endAge, 
                            generator,
                            lowerBoundBehaviour,
                            upperBoundBehaviour) =
    
            member private this.boundary boundaryAge = function
                | Zero v -> v
                | One v -> v
                | Fixed(v) -> v
                | Extend -> this.AtAge boundaryAge
                | Fail -> failwith "Requested Age is out of bounds and no substitute value has been declared."
    
            member this.AtAge age = (this :> IAgeVector<'T>).ValueAtAge age
    
            interface IAgeVector<'T> with
                member this.StartAge with get () = startAge
                member this.EndAge with get () = endAge
                member this.ValueAtAge age =
                    match age with
                    | ValidAge v when v < startAge -> this.boundary startAge lowerBoundBehaviour
                    | ValidAge v when v > endAge -> this.boundary endAge upperBoundBehaviour
                    | ValidAge v -> generator v
                    | _ -> failwith "Invalid age."
                member this.LowerBoundBehaviour with get() = lowerBoundBehaviour
                member this.UpperBoundBehaviour with get() = upperBoundBehaviour
 
            new (startAge, 
                 endAge,
                 data : seq<'T>,
                 lowerBoundBehaviour,
                 upperBoundBehaviour) =
                let generator (age : Age) = 
                    data 
                    |> Seq.nth (age - startAge)
                new AgeVector<'T> (startAge, 
                                   endAge, 
                                   generator,
                                   lowerBoundBehaviour,
                                   upperBoundBehaviour)


        // ***************************************************************
        // Implement builder logic
        // ***************************************************************
        let bind (av : AgeVector<'T>) (rest : (Age -> 'T) -> AgeVector<'U>) : AgeVector<'U> = rest av.AtAge

        type AgeVectorBuilder<'T>(startAge : Age,
                                  endAge : Age,
                                  lowerBoundBehaviour : boundaryBehaviour<'T>,
                                  upperBoundBehaviour : boundaryBehaviour<'T>) =
            member this.StartAge with get () = startAge
            member this.EndAge with get () = endAge
            member this.LowerBoundBehaviour with get () = lowerBoundBehaviour
            member this.UpperBoundBehaviour with get () = upperBoundBehaviour

            member this.Delay(f) = f()
            member this.Return (genFunc : Age -> 'T) = 
                new AgeVector<'T>(startAge, endAge, genFunc, lowerBoundBehaviour, upperBoundBehaviour)
            member this.ReturnFrom(genFunc : Age -> 'T) = genFunc
            member this.Bind (av, rest) = bind av rest
            member this.Let (av, rest) : AgeVector<'T> = rest av        
    
        let defaultAgeVector = new AgeVectorBuilder<_>(18, 120, Zero (0.0), Fail)

    // ***************************************************************
    module Utility =
        open AgeVector

        let probSurvival ageVectorFn (term : Term) =
            let psFunc (age : Age) = 
                [age .. (age + term - 1)]
                |> List.fold (fun acc age -> acc * (1.0 - (ageVectorFn age))) 1.0
            psFunc

        let discount pensionIncr intr (term : Term) =
            ((1.0 + pensionIncr) / (1.0 + intr)) ** (double)term

        let pureEndowment (psFunc : Term -> (Age -> double)) (discountToTerm : Term -> (double -> double)) = 
            fun term -> (psFunc term) >> (discountToTerm  term)
        
        let transform f ageVector =
            let genFunc = f << (ageVector :> IAgeVector<_>).ValueAtAge
            let newAgeVector = new AgeVector<_> (
                                    ageVector.StartAge,
                                    ageVector.EndAge,
                                    genFunc,
                                    ageVector.LowerBoundBehaviour,
                                    ageVector.UpperBoundBehaviour)
            newAgeVector

    module ActuarialFunction =
            open System
        
            let annuityCertain interest pensionIncrease = function
                | 0 -> 0.0
                | term when interest = pensionIncrease -> (double)term
                | term -> let scaling = Math.Log (Utility.discount pensionIncrease interest 1)
                          ((Utility.discount pensionIncrease interest term) - 1.0) / scaling 
