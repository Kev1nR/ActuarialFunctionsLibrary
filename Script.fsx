// This file is a script that can be executed with the F# Interactive.  
// It can be used to explore and test the library project.
// Note that script files will not be part of the project build.

#load "ActuarialFunctionLibrary.fs"

open ActuarialFunctionLibrary.AgeVector
open ActuarialFunctionLibrary.Utility

let x = 1
//module SomeTests =
//
//        // *******************************************************************
//        // Test data - mortality table from 18 - 120. This is simply an extract 
//        // of the publicly available PMA92 (C=2003) mortality table.
//        // *******************************************************************
//        let pma92vals = 
//            [0.00;0.00;0.000235;0.000233;0.000233;0.000231;0.000231;0.000230;
//             0.000229;0.000229;0.000229;0.000229;0.000230;0.000231;0.000233;
//             0.000237;0.000241;0.000247;0.000254;0.000262;0.000274;0.000288;
//             0.000306;0.000328;0.000355;0.000388;0.000428;0.000476;0.000535;
//             0.000605;0.000689;0.000789;0.000908;0.001049;0.001216;0.001413;
//             0.001643;0.001914;0.00223;0.002597;0.003023;0.003516;0.004085;
//             0.004806;0.005642;0.00661;0.007725;0.009006;0.010474;0.012149;
//             0.014054;0.016214;0.018653;0.021399;0.024479;0.027922;0.031756;
//             0.03601;0.040712;0.04589;0.051571;0.05778;0.064539;0.071867;
//             0.079782;0.088295;0.097414;0.107142;0.117477;0.128409;0.139923;
//             0.151999;0.164609;0.177718;0.191285;0.205265;0.219604;0.234247;
//             0.24913;0.264188;0.279353;0.294553;0.309716;0.32477;0.339641;
//             0.35426;0.368556;0.382461;0.395911;0.408847;0.421211;0.432949;
//             0.444014;0.453033;0.461297;0.46878;0.475459;0.481313;0.486326;
//             0.490484;0.493776;0.496194;1.0]
//
//        //// some Tests
//        let discFunc term = fun ps -> (AgeVectorFunctions.discount 0.02 0.03 term) * ps
//        let pma92 = new AgeVector<double>(18, 120, pma92vals, Extend, Extend)
//
//        let simpleScaling = defaultAgeVector {
//                let halveIt = fun dblVal -> dblVal*0.5
//                let! pma92fn = pma92
//                return (pma92fn >> halveIt)}
//
//        let simpleShift n = defaultAgeVector {
//                let! pma92fn = pma92
//                return (fun age -> pma92fn (age - n))}
//
//        let singleLifeAnnuity = defaultAgeVector {
//                let ea = defaultAgeVector.EndAge
//                let! pma92fn = pma92
//                let psFn = fun term -> AgeVectorFunctions.probSurvival pma92fn term
//                let asl = fun age ->
//                    [1..(ea - age)]
//                    |> List.fold  (fun acc a -> 
//                            let pe = (AgeVectorFunctions.pureEndowment (psFn) discFunc a) age
//                            acc + (AgeVectorFunctions.pureEndowment (psFn) discFunc a) age) 0.0
//         
//                return (asl)
//            }
//
//
//        let pfa92vals = [0.0;0.0;0.0;0.0;0.0;0.0;0.0;0.0;0.0;0.0;0.0;0.0;0.0;0.0;0.0;
//                         0.0;0.0;0.0;0.0;0.00014;0.00014;0.00014;
//                         0.00014;0.00014;0.00014;0.000141;0.000142;0.000144;0.000146;
//                         0.00015;0.000153;0.000159;0.000164;0.000172;0.000182;0.000193;
//                         0.000206;0.000222;0.000241;0.000264;0.000291;0.000323;0.000361;
//                         0.000405;0.000457;0.000518;0.000589;0.000671;0.000767;0.000878;
//                         0.001006;0.001154;0.001324;0.00152;0.001744;0.001999;0.002291;
//                         0.002624;0.003001;0.00343;0.003969;0.004585;0.005287;0.006084;
//                         0.006989;0.008012;0.009166;0.010465;0.011924;0.013557;0.015381;
//                         0.017413;0.019671;0.022174;0.024942;0.027994;0.03135;0.035029;
//                         0.039055;0.043445;0.048222;0.053402;0.059006;0.065049;0.071549;
//                         0.078518;0.085968;0.093911;0.10235;0.111291;0.120735;0.130678;
//                         0.141114;0.152032;0.163418;0.175253;0.187516;0.20018;0.213215;
//                         0.226587;0.240259;0.254192;0.26834;0.282658;0.2971;0.311616;
//                         0.326156;0.340667;0.3551;0.369403;0.382406;0.39515;0.407594;
//                         0.4197;0.431431;0.442752;0.45363;0.464038;0.473947;1.0]
//
//        // Joint life annuity same age assumed
//        let jointLifeAnnuity = defaultAgeVector {
//                let sa = defaultAgeVector.StartAge
//                let ea = defaultAgeVector.EndAge
//                let lb = defaultAgeVector.LowerBoundBehaviour
//                let ub = defaultAgeVector.UpperBoundBehaviour
//                let! pma92fn = pma92
//                let! pfa92fn = new AgeVector<double>(1, ea, pfa92vals, lb, ub)
//                let psFn = fun term -> 
//                    fun age -> ((AgeVectorFunctions.probSurvival pma92fn term) age)*
//                               ((AgeVectorFunctions.probSurvival pfa92fn term) age)
//                let asl = fun age ->
//                    [1..(ea - age)]
//                    |> List.fold  (fun acc a -> 
//                            acc + (AgeVectorFunctions.pureEndowment (psFn) discFunc a) age) 0.0
//         
//                return (asl)
//            }
//
//        // Joint life with age difference
//        let ageDiffedSpouse n = defaultAgeVector {
//                let sa = defaultAgeVector.StartAge
//                let ea = defaultAgeVector.EndAge
//                let lb = defaultAgeVector.LowerBoundBehaviour
//                let ub = defaultAgeVector.UpperBoundBehaviour
//                let! pfa92fn = new AgeVector<double>(1, ea, pfa92vals, lb, ub)
//                return (fun age -> pfa92fn (age - n))
//            }
//
//        let jointLifeAnnuityAgeDiff = defaultAgeVector {
//                let sa = defaultAgeVector.StartAge
//                let ea = defaultAgeVector.EndAge
//                let lb = defaultAgeVector.LowerBoundBehaviour
//                let ub = defaultAgeVector.UpperBoundBehaviour
//                let! pma92fn = pma92
//                let! pfa92fn = ageDiffedSpouse 3
//                let psFn = fun term -> 
//                    fun age -> ((AgeVectorFunctions.probSurvival pma92fn term) age)*
//                                ((AgeVectorFunctions.probSurvival pfa92fn term) age)
//                let asl = fun age ->
//                    [1..(ea - age)]
//                    |> List.fold  (fun acc a -> 
//                            acc + (AgeVectorFunctions.pureEndowment (psFn) discFunc a) age) 0.0
//         
//                return (asl)
//            }
//    
//
