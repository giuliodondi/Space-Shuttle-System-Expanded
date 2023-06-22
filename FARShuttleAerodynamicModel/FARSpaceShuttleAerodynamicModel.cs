

using System;
using System.Linq;
using FerramAerospaceResearch;
using static FinePrint.ContractDefs;

// ReSharper disable once CheckNamespace
namespace ferram4
{
    /// <summary>
    ///     This calculates the lift and drag on a wing in the atmosphere
    ///     It uses Prandtl lifting line theory to calculate the basic lift and drag coefficients and includes compressibility
    ///     corrections for subsonic and supersonic flows; transonic regime has placeholder
    /// </summary>
    public class FARSpaceShuttleAerodynamicModel : FARWingAerodynamicModel
    {

        private double radiansToDegrees = (180 / Math.PI);

        /*
        coefficients taken from "OPERATIONAL AERODYNAMIC DATA BOOK VOL1 -   SEPTEMBER 1985
         */

        private double[] shuttleAoAArr = { -10, -5, -2.5, 0, 2.5, 5, 7.5, 10, 12.5, 15, 17.5, 20, 22.5, 25, 30, 35, 40, 45, 50};

        private double[] shuttleMachArr = { 0.25, 0.4, 0.6, 0.8, 0.85, 0.9, 0.92, 0.95, 0.98, 1.05, 1.1, 1.2, 1.3, 1.5, 2, 2.5, 3, 4, 5, 8, 10, 15, 20, 25, 30};


        private double maxOverridableAoA = 50 * Math.PI / 180;
        private double minOverridableAoA = -10 * Math.PI / 180;

        private double maxOverridableMach = 30;
        private double minOverridableMach = 0.25;

        private double[,] shuttleClTable =
        {
            {-0.5028,-0.25906,-0.15202,-0.05,0.06214,0.17453,0.29705,0.40448,0.51599,0.63581,0.77676,0.91657,1.00634,1.08964,1.08964,1.08964,1.08964,1.08964,1.08964},
            {-0.51097,-0.27397,-0.16199,-0.05,0.06212,0.17448,0.29691,0.40924,0.52757,0.65002,0.78385,0.92529,1.01512,1.08846,1.08846,1.08846,1.08846,1.08846,1.08846},
            {-0.52957,-0.2887,-0.17188,-0.055,0.07201,0.17928,0.30657,0.41674,0.54905,0.67426,0.79709,0.92851,1.01007,1.04489,1.04489,1.04489,1.04489,1.04489,1.04489},
            {-0.58454,-0.32751,-0.19155,-0.065,0.06176,0.18859,0.31963,0.44384,0.5648,0.68162,0.79087,0.89578,0.93184,0.92299,0.92299,0.92299,0.92299,0.92299,0.92299},
            {-0.59773,-0.3379,-0.19632,-0.062,0.06652,0.19295,0.31838,0.44172,0.56565,0.67309,0.77995,0.8769,0.91266,0.90795,0.90795,0.90795,0.90795,0.90795,0.90795},
            {-0.61005,-0.34601,-0.20588,-0.06,0.07101,0.20356,0.32413,0.44572,0.56794,0.68323,0.79401,0.89485,0.92074,0.93837,0.93837,0.93837,0.93837,0.93837,0.93837},
            {-0.62831,-0.35039,-0.21553,-0.06,0.07563,0.21579,0.34997,0.4769,0.58585,0.70567,0.81577,0.91619,0.95994,0.96751,0.96751,0.96751,0.96751,0.96751,0.96751},
            {-0.65514,-0.35897,-0.21485,-0.06,0.08498,0.23947,0.38783,0.51887,0.6466,0.76535,0.86501,0.96429,1.00227,1.02232,1.02232,1.02232,1.02232,1.02232,1.02232},
            {-0.65616,-0.36172,-0.19399,-0.055,0.09414,0.24762,0.38955,0.52999,0.66026,0.77783,0.89069,0.99805,1.04837,1.07579,1.07579,1.07579,1.07579,1.07579,1.07579},
            {-0.64808,-0.36027,-0.2031,-0.047,0.10315,0.25552,0.39173,0.53602,0.66393,0.78377,0.89596,1.00286,1.06699,1.10328,1.10328,1.10328,1.10328,1.10328,1.10328},
            {-0.64225,-0.35685,-0.19292,-0.036,0.10796,0.25513,0.39121,0.53054,0.66357,0.77863,0.89086,1.00255,1.0656,1.11188,1.11188,1.11188,1.11188,1.11188,1.11188},
            {-0.61158,-0.33449,-0.18275,-0.025,0.1046,0.25014,0.38627,0.51767,0.63927,0.75952,0.85608,0.95636,1.03541,1.09985,1.09985,1.09985,1.09985,1.09985,1.09985},
            {-0.57681,-0.30454,-0.16776,-0.023,0.10295,0.24043,0.37178,0.49146,0.61835,0.72557,0.83073,0.92538,1.00114,1.07176,1.07176,1.07176,1.07176,1.07176,1.07176},
            {-0.48844,-0.25188,-0.12989,-0.015,0.09317,0.21606,0.32805,0.42874,0.54671,0.64381,0.74311,0.83834,0.91858,0.99205,0.99205,0.99205,0.99205,0.99205,0.99205},
            {-0.36885,-0.19695,-0.09888,-0.017,0.08419,0.16125,0.24173,0.33103,0.41958,0.50777,0.60244,0.68871,0.76904,0.84561,0.99835,0.99835,0.99835,0.99835,0.99835},
            {-0.31514,-0.16705,-0.10456,-0.0429,0.02995,0.10458,0.18906,0.27135,0.35229,0.43483,0.52033,0.60168,0.67886,0.76551,0.91867,0.91867,0.91867,0.91867,0.91867},
            {-0.29383,-0.16922,-0.10504,-0.037,0.02048,0.085,0.15609,0.22528,0.29513,0.36604,0.44227,0.52215,0.60751,0.68386,0.84307,0.97219,1.07919,1.15575,1.15575},
            {-0.22874,-0.13893,-0.1005,-0.0443,0.01111,0.06174,0.11881,0.18185,0.2473,0.31897,0.39229,0.46559,0.53159,0.61884,0.76946,0.8953,1.00341,1.07904,1.07904},
            {-0.19854,-0.12424,-0.09081,-0.04724,0.00542,0.04301,0.09656,0.1522,0.21351,0.27892,0.35079,0.42351,0.49654,0.57019,0.72044,0.84607,0.96129,1.03683,1.08629},
            {-0.18641,-0.12402,-0.08913,-0.0554,-0.02023,0.01774,0.05965,0.10504,0.16115,0.22632,0.29996,0.36492,0.44356,0.51805,0.66845,0.79881,0.91017,0.98747,1.03614},
            {-0.18641,-0.12402,-0.08913,-0.0554,-0.02023,0.01754,0.05558,0.10002,0.15328,0.21732,0.29043,0.35587,0.43474,0.50936,0.64465,0.78362,0.884,0.96803,1.02007},
            {-0.18641,-0.12402,-0.08913,-0.0554,-0.02023,0.01754,0.05558,0.10002,0.15328,0.21732,0.29043,0.35587,0.43474,0.50935,0.64465,0.78382,0.884,0.96803,1.02007},
            {-0.18541,-0.12402,-0.08913,-0.0554,-0.02023,0.01754,0.05558,0.10002,0.15328,0.21732,0.29043,0.35587,0.43474,0.50936,0.64455,0.78382,0.884,0.96803,1.02007},
            {-0.18541,-0.12402,-0.08913,-0.0554,-0.02023,0.01754,0.05558,0.10002,0.15328,0.21732,0.29043,0.35587,0.43474,0.50936,0.64455,0.78382,0.884,0.96803,1.02007},
            { -0.18541,-0.12402,-0.08913,-0.0554,-0.02023,0.01754,0.05558,0.10002,0.15328,0.21732,0.29043,0.35587,0.43474,0.50936,0.64455,0.78382,0.884,0.96803,1.02007}
        };

        private double[,] shuttleCdTable =
        {
            {0.11485,0.07985,0.0717,0.0684,0.06687,0.07038,0.08056,0.09599,0.12125,0.15784,0.21346,0.28859,0.36651,0.45537,0.45537,0.45537,0.45537,0.45537,0.45537},
            {0.11975,0.08119,0.07263,0.0681,0.06747,0.07098,0.08145,0.09775,0.12944,0.16278,0.21758,0.29389,0.37144,0.45793,0.45793,0.45793,0.45793,0.45793,0.45793},
            {0.13521,0.08489,0.07517,0.0702,0.07011,0.0735,0.08464,0.10146,0.13381,0.18632,0.26837,0.35519,0.43592,0.50401,0.50401,0.50401,0.50401,0.50401,0.50401},
            {0.16897,0.08953,0.08323,0.0763,0.07557,0.08175,0.10038,0.13188,0.17827,0.23303,0.30939,0.38957,0.45493,0.50539,0.50539,0.50539,0.50539,0.50539,0.50539},
            {0.18,0.10786,0.08675,0.082,0.08318,0.08916,0.1099,0.14389,0.1919,0.25055,0.31973,0.39759,0.46803,0.51375,0.51375,0.51375,0.51375,0.51375,0.51375},
            {0.19703,0.11831,0.09887,0.0907,0.09309,0.10579,0.1275,0.16155,0.21031,0.27056,0.34158,0.42137,0.48172,0.54316,0.54316,0.54316,0.54316,0.54316,0.54316},
            {0.20287,0.12552,0.1071,0.0998,0.1018,0.11515,0.13806,0.17477,0.22198,0.2836,0.35877,0.43584,0.50467,0.56348,0.56348,0.56348,0.56348,0.56348,0.56348},
            {0.23925,0.14223,0.12269,0.1145,0.11702,0.13127,0.15896,0.19583,0.24923,0.31191,0.38252,0.46452,0.53313,0.59974,0.59974,0.59974,0.59974,0.59974,0.59974},
            {0.25227,0.16817,0.14461,0.1346,0.13644,0.15286,0.18221,0.22487,0.27902,0.34259,0.41746,0.50331,0.57664,0.65071,0.65071,0.65071,0.65071,0.65071,0.65071},
            {0.26933,0.1847,0.16251,0.1541,0.15935,0.17724,0.20398,0.24255,0.29479,0.35909,0.43401,0.51932,0.59902,0.67457,0.67457,0.67457,0.67457,0.67457,0.67457},
            {0.27358,0.18942,0.16647,0.1584,0.16376,0.18173,0.20794,0.24485,0.29645,0.35895,0.43355,0.52016,0.59994,0.67979,0.67979,0.67979,0.67979,0.67979,0.67979},
            {0.27477,0.19259,0.17013,0.1514,0.16502,0.18139,0.20709,0.24298,0.29055,0.35301,0.42081,0.5009,0.56377,0.6701,0.6701,0.6701,0.6701,0.6701,0.6701},
            {0.27037,0.19067,0.16958,0.1611,0.16395,0.17763,0.20226,0.23613,0.29325,0.34037,0.40809,0.48367,0.552,0.64751,0.64751,0.64751,0.64751,0.64751,0.64751},
            {0.25326,0.18445,0.16572,0.1577,0.15852,0.16938,0.18964,0.21755,0.25989,0.30958,0.37008,0.43922,0.51308,0.59368,0.59368,0.59368,0.59368,0.59368,0.59368},
            {0.21167,0.15837,0.14245,0.1349,0.135,0.14189,0.15579,0.1765,0.20958,0.24932,0.30079,0.35911,0.42462,0.49836,0.67501,0.67501,0.67501,0.67501,0.67501},
            {0.18959,0.141,0.12598,0.1186,0.11632,0.12068,0.13301,0.15244,0.17951,0.21476,0.2599,0.31286,0.3732,0.447,0.61723,0.61723,0.61723,0.61723,0.61723},
            {0.1764,0.13105,0.11599,0.1068,0.10399,0.107,0.11677,0.13324,0.15649,0.18711,0.22579,0.27582,0.33639,0.40275,0.56977,0.76472,0.99301,1.24741,1.24741},
            {0.15782,0.11935,0.10538,0.0943,0.08937,0.09063,0.09815,0.11238,0.1338,0.16342,0.20107,0.24672,0.29758,0.36614,0.52265,0.70975,0.92759,1.16885,1.16885},
            {0.14843,0.11165,0.09795,0.0876,0.08226,0.08096,0.08634,0.09853,0.1176,0.14482,0.18106,0.22523,0.27765,0.33893,0.49216,0.67287,0.89082,1.1255,1.38996},
            {0.14122,0.10501,0.09067,0.08,0.07359,0.07142,0.07402,0.08216,0.0978,0.12245,0.15697,0.19614,0.24878,0.30876,0.45621,0.63575,0.8444,1.07445,1.33019},
            {0.14122,0.10501,0.09067,0.08,0.07359,0.0714,0.07348,0.08059,0.09544,0.11931,0.15302,0.19178,0.24394,0.30372,0.44343,0.62404,0.82113,1.0543,1.31104},
            {0.14122,0.10501,0.09067,0.08,0.07359,0.0714,0.07348,0.08059,0.09544,0.11931,0.15302,0.19178,0.24394,0.30372,0.44343,0.62404,0.82113,1.0543,1.31104},
            {0.14122,0.10501,0.09067,0.08,0.07359,0.0714,0.07348,0.08059,0.09544,0.11931,0.15302,0.19178,0.24394,0.30372,0.44343,0.62404,0.82113,1.0543,1.31104},
            {0.14122,0.10501,0.09067,0.08,0.07359,0.0714,0.07348,0.08059,0.09544,0.11931,0.15302,0.19178,0.24394,0.30372,0.44343,0.62404,0.82113,1.0543,1.31104},
            {0.14122,0.10501,0.09067,0.08,0.07359,0.0714,0.07348,0.08059,0.09544,0.11931,0.15302,0.19178,0.24394,0.30372,0.44343,0.62404,0.82113,1.0543,1.31104 }
        };


        protected override Vector3d CalculateAerodynamicCenter(double MachNumber, double AoA, Vector3d WC)
        {
            //very brutal
            return WC;
        }

        protected override void CalculateCoefficients(double MachNumber, double AoA, double skinFrictionCoefficient)
        {
            minStall = 0;

            rawLiftSlope = CalculateSubsonicLiftSlope(MachNumber); // / AoA;     //Prandtl lifting Line


            CalculateWingCamberInteractions(MachNumber, AoA, out double ACshift, out double ACweight);
            DetermineStall(AoA);

            double beta = Math.Sqrt(MachNumber * MachNumber - 1);
            if (double.IsNaN(beta) || beta < 0.66332495807107996982298654733414)
                beta = 0.66332495807107996982298654733414;

            double TanSweep = Math.Sqrt((1 - cosSweepAngle * cosSweepAngle).Clamp(0, 1)) / cosSweepAngle;
            double beta_TanSweep = beta / TanSweep;


            double Cd0 = CdCompressibilityZeroLiftIncrement(MachNumber, cosSweepAngle, TanSweep, beta_TanSweep, beta) +
                         2 * skinFrictionCoefficient;
            double CdMax = CdMaxFlatPlate(MachNumber, beta);
            e = FARAeroUtil.CalculateOswaldsEfficiencyNitaScholz(effective_AR, cosSweepAngle, Cd0, TaperRatio);
            piARe = effective_AR * e * Math.PI;

            double CosAoA = Math.Cos(AoA);

            if (MachNumber <= 0.8)
            {
                double Cn = liftslope;
                FinalLiftSlope = liftslope;
                double sinAoA = Math.Sqrt((1 - CosAoA * CosAoA).Clamp(0, 1));
                Cl = Cn * CosAoA * Math.Sign(AoA);

                Cl += ClIncrementFromRear;
                Cl *= sinAoA;

                if (Math.Abs(Cl) > Math.Abs(ACweight))
                    ACshift *= Math.Abs(ACweight / Cl).Clamp(0, 1);
                Cd = Cl * Cl / piARe; //Drag due to 3D effects on wing and base constant
                Cd += Cd0;
            }
            /*
             * Supersonic nonlinear lift / drag code
             *
             */
            else if (MachNumber > 1.4)
            {
                double coefMult = 2 / (FARAeroUtil.CurrentAdiabaticIndex * MachNumber * MachNumber);

                double supersonicLENormalForceFactor = CalculateSupersonicLEFactor(beta, TanSweep, beta_TanSweep);

                double normalForce = GetSupersonicPressureDifference(MachNumber, AoA);
                FinalLiftSlope = coefMult * normalForce * supersonicLENormalForceFactor;

                Cl = FinalLiftSlope * CosAoA * Math.Sign(AoA);
                Cd = beta * Cl * Cl / piARe;

                Cd += Cd0;
            }
            /*
             * Transonic nonlinear lift / drag code
             * This uses a blend of subsonic and supersonic aerodynamics to try and smooth the gap between the two regimes
             */
            else
            {
                //This determines the weight of supersonic flow; subsonic uses 1-this
                double supScale = 2 * MachNumber;
                supScale -= 6.6;
                supScale *= MachNumber;
                supScale += 6.72;
                supScale *= MachNumber;
                supScale += -2.176;
                supScale *= -4.6296296296296296296296296296296;

                double Cn = liftslope;
                double sinAoA = Math.Sqrt((1 - CosAoA * CosAoA).Clamp(0, 1));
                Cl = Cn * CosAoA * sinAoA * Math.Sign(AoA);

                if (MachNumber <= 1)
                {
                    Cl += ClIncrementFromRear * sinAoA;
                    if (Math.Abs(Cl) > Math.Abs(ACweight))
                        ACshift *= Math.Abs(ACweight / Cl).Clamp(0, 1);
                }

                FinalLiftSlope = Cn * (1 - supScale);
                Cl *= 1 - supScale;

                double M = MachNumber.Clamp(1.2, double.PositiveInfinity);

                double coefMult = 2 / (FARAeroUtil.CurrentAdiabaticIndex * M * M);

                double supersonicLENormalForceFactor = CalculateSupersonicLEFactor(beta, TanSweep, beta_TanSweep);

                double normalForce = GetSupersonicPressureDifference(M, AoA);

                double supersonicLiftSlope = coefMult * normalForce * supersonicLENormalForceFactor * supScale;
                FinalLiftSlope += supersonicLiftSlope;


                Cl += CosAoA * Math.Sign(AoA) * supersonicLiftSlope;

                double effectiveBeta = beta * supScale + (1 - supScale);

                Cd = effectiveBeta * Cl * Cl / piARe;

                Cd += Cd0;
            }

            //do not override unless AoA is acceptable
            if (minOverridableAoA <= AoA && AoA <= maxOverridableAoA && minOverridableMach <= MachNumber && MachNumber <= maxOverridableMach) {
                double[] shuttleCoeffs = getSpaceShuttleCoeffs(MachNumber, AoA);
                Cl = shuttleCoeffs[0];
                Cd = shuttleCoeffs[1];
            }

            //AC shift due to flaps
            Vector3d ACShiftVec;
            if (!double.IsNaN(ACshift) && MachNumber <= 1)
                ACShiftVec = ACshift * ParallelInPlane;
            else
                ACShiftVec = Vector3d.zero;

            //Stalling effects
            stall = stall.Clamp(minStall, 1);

            //AC shift due to stall
            if (stall > 0)
                ACShiftVec -= 0.75 / criticalCl * MAC_actual * Math.Abs(Cl) * stall * ParallelInPlane * CosAoA;

            Cl -= Cl * stall * 0.769;
            Cd += Cd * stall * 3;
            //Cd = Math.Max(Cd, CdMax * (1 - CosAoA * CosAoA));

            Cd = Math.Max(Cd, 0.01);    //artificial

            AerodynamicCenter += ACShiftVec;

            Cl *= wingInteraction.ClInterferenceFactor;

            FinalLiftSlope *= wingInteraction.ClInterferenceFactor;

            ClIncrementFromRear = 0;

            double shuttleLD = Cl / Cd;
           // UnityEngine.Debug.Log(String.Format("Shuttle Mach, Aoa, Cl, Cd, LD :  {0}, {1}, {2}, {3}, {4}", MachNumber, AoA * radiansToDegrees, Cl, Cd, shuttleLD));

        }

        private double[] getSpaceShuttleCoeffs(double mach, double aoaRad)
        {
            double aoa = aoaRad * radiansToDegrees;

            int m_idx = 0;
            for (int i = 1; i < shuttleMachArr.Length; i++)
            {
                if (shuttleMachArr[i] >= mach)
                {
                    m_idx = i - 1;
                    break;
                }
            }

            int a_idx = 0;
            for (int i = 1; i < shuttleAoAArr.Length; i++)
            {
                if (shuttleAoAArr[i] >= aoa)
                {
                    a_idx = i - 1;
                    break;
                }
            }

            double aoa1 = shuttleAoAArr[a_idx];
            double aoa2 = shuttleAoAArr[a_idx + 1];

            double mach1 = shuttleMachArr[m_idx];
            double mach2 = shuttleMachArr[m_idx + 1];

            double daoa = aoa2 - aoa1;
            double dmach = mach2 - mach1;

            double del_aoa = aoa - aoa1;
            double del_mach = mach - mach1;

            double Cl1 = shuttleClTable[m_idx, a_idx];
            double Cl2 = shuttleClTable[m_idx, a_idx + 1];
            double Cl3 = shuttleClTable[m_idx + 1, a_idx];
            double Cl4 = shuttleClTable[m_idx + 1, a_idx + 1];

            double Cla1 = ((Cl3 - Cl1) / dmach) * del_mach + Cl1;
            double Cla2 = ((Cl4 - Cl2) / dmach) * del_mach + Cl2;

            double Cl = ((Cla2 - Cla1) / daoa) * del_aoa + Cla1;

            double Cd1 = shuttleCdTable[m_idx, a_idx];
            double Cd2 = shuttleCdTable[m_idx, a_idx + 1];
            double Cd3 = shuttleCdTable[m_idx + 1, a_idx];
            double Cd4 = shuttleCdTable[m_idx + 1, a_idx + 1];

            double Cda1 = ((Cd3 - Cd1) / dmach) * del_mach + Cd1;
            double Cda2 = ((Cd4 - Cd2) / dmach) * del_mach + Cd2;

            double Cd = ((Cda2 - Cda1) / daoa) * del_aoa + Cda1;

            /*
                Apply corrections so that the overall Shuttle Orbiter L/D is close to what it should be
                Measure cl and cd using the FAR static analysis, build a csv with the deviations from the correct ones
                and fit them with an order 1 2-dim polynomial
                Fudge factor for fine adjustments
             */ 
            double fudgeCl = 0.52;
            double fudgeCd = 0.7;

            double Clf = Cl + fudgeCl * (-0.013305163770468177 + 0.0008416398153846159 * aoa + -0.0027519394785337293 * mach);

            double Cdf = Cd + fudgeCd * (0.028248385693693485 + 0.00022861077472527479 * aoa + -0.008744485572793892 * mach);  

            //UnityEngine.Debug.Log(String.Format("aoa, mach, aoa1, aoa2, mach1, mach2, {0}, {1}, {2}, {3}, {4}, {5}", aoa, mach, aoa1, aoa2, mach1, mach2));
            //UnityEngine.Debug.Log(String.Format("cl1, cl2, cl3, cl4, {0}, {1}, {2}, {3}", Cl1, Cl2, Cl3, Cl4));
            //UnityEngine.Debug.Log(String.Format("cla1, cla2, clf, cl, {0}, {1}, {2} ", Cla1, Cla2, Cl));


            return new double[] { Clf, Cdf };
        }

        private double getSpaceShuttleCL(double mach, double aoa)
        {
            double aoa1 = aoa * radiansToDegrees;
            double mach1 = Math.Min(mach, 12);
            ;

            double mach2 = mach1 * mach1;
            double mach3 = mach2 * mach1;
            double mach4 = mach3 * mach1;
            double aoa2 = aoa1 * aoa1;
            double aoa3 = aoa2 * aoa1;
            double aoa4 = aoa3 * aoa1;

            double C_l = 0d;

            /*
                coefficients taken from "OPERATIONAL AERODYNAMIC DATA BOOK VOL1 -   SEPTEMBER 1985
                then fitted with a 4-order 2-dimensional polynomial
             */

            if (mach1 <= 0.98)
            {

                C_l = 0.41674050087360154
                    + 0.022618014309512165 * aoa1
                    + -3.8517309823544585 * mach1
                    - 3.447831829016871e-05 * aoa2
                    + 0.12060419227614055 * aoa1 * mach1
                    + 10.913375351864904 * mach2
                    + 4.1941053077128396e-05 * aoa3
                    + 0.0007256328506747937 * aoa2 * mach1
                    + -0.21769823895771614 * aoa1 * mach2
                    + -12.789664535117232 * mach3
                    + -1.2028860312089662e-06 * aoa4
                    + -3.604118399909103e-05 * aoa3 * mach1
                    + -0.000736715236832555 * aoa2 * mach2
                    + 0.13553398139467013 * aoa1 * mach3
                    + 5.276229800620859 * mach4;

            }
            else if (mach1 <= 1.5)
            {
                C_l = -16.819656903812113
                    - 0.37557314473284936 * aoa1
                    + 57.698403805944686 * mach1
                    + 0.001357468605737764 * aoa2
                    + 1.0643874346141937 * aoa1 * mach1
                    + -73.92901691681138 * mach2
                    + -4.2362698091168854e-05 * aoa3
                    + -0.0022454007377806394 * aoa2 * mach1
                    + -0.8420929486627748 * aoa1 * mach2
                    + 41.76093242619697 * mach3
                    + -5.252171156322483e-07 * aoa4
                    + 3.421287866051809e-05 * aoa3 * mach1
                    + 0.0008506207411898664 * aoa2 * mach2
                    + 0.213049349942749 * aoa1 * mach3
                    + -8.759814225365204 * mach4;
            }
            else if (mach1 <= 5.0)
            {
                C_l = -0.28071051192647845
                    + 0.08827327647680949 * aoa1
                    + 0.5041991858603336 * mach1
                    + -0.0012763617040020333 * aoa2
                    + -0.028499783109406058 * aoa1 * mach1
                    + -0.3193334962582776 * mach2
                    + -6.186950947147888e-06 * aoa3
                    + 0.0008103101062832149 * aoa2 * mach1
                    + 0.0020164030808757587 * aoa1 * mach2
                    + 0.07828103903561803 * mach3
                    + 1.484892289266293e-07 * aoa4
                    + -3.3770608495881094e-06 * aoa3 * mach1
                    + -7.581519410788576e-05 * aoa2 * mach2
                    + 0.0001731979581358646 * aoa1 * mach3
                    + -0.006608657654115985 * mach4;

            }
            else
            {
                C_l = 0.14244079143282407
                    + 0.04081440740058968 * aoa1
                    + -0.1044662793562203 * mach1
                    + -1.664435626497059e-05 * aoa2
                    + -0.005856699721226352 * aoa1 * mach1
                    + 0.017590166144463387 * mach2
                    + 3.7150947137358243e-06 * aoa3
                    + 5.501128747772014e-05 * aoa2 * mach1
                    + 0.00038859096173361014 * aoa1 * mach2
                    + -0.0011597400892722812 * mach3
                    + -1.279398050904601e-07 * aoa4
                    + -1.2058275905817845e-07 * aoa3 * mach1
                    + -1.7318276701980446e-06 * aoa2 * mach2
                    + -8.097909353795268e-06 * aoa1 * mach3
                    + 2.5759480535510453e-05 * mach4;
            }


            /*
            correction to account for control surfaces and the oms pods
            let FAR predict the final Cl and Cd, extracted the deviations, and fitter with a 2 degree polynomial
             */

            //C_l += -0.0050669221306456555
            //    + -0.003932365617148483 * aoa1
            //    + 0.0006806054816500566 * mach1
            //    + 1.163725233100342e-05 * aoa2
            //    + 4.1076860645417046e-05 * aoa1 * mach1
            //    + -1.7441330491309217e-05 * mach2;

            //C_l += -0.010507464824334019 + -0.0032168273427972036 * aoa1 * +0.0009863247345228707 * mach1;

            return C_l;
        }

        private double getSpaceShuttleCd(double mach, double aoa)
        {
            double aoa1 = aoa * radiansToDegrees;
            double mach1 = Math.Min(mach, 12);

            double mach2 = mach1 * mach1;
            double mach3 = mach2 * mach1;
            double mach4 = mach3 * mach1;
            double aoa2 = aoa1 * aoa1;
            double aoa3 = aoa2 * aoa1;
            double aoa4 = aoa3 * aoa1;

            double C_d = 0d;

            /*
                coefficients taken from "OPERATIONAL AERODYNAMIC DATA BOOK VOL1 -   SEPTEMBER 1985
                then fitted with a 4-order 2-dimensional polynomial
             */

            if (mach1 <= 0.98)
            {
                C_d = 0.36773376220150766
                    + -0.007420266032914213 * aoa1
                    + -2.480479910682402 * mach1
                    + 4.5350631137655685e-05 * aoa2
                    + 0.021600092799941472 * aoa1 * mach1
                    + 7.165733465935746 * mach2
                    + 4.1982088232892434e-05 * aoa3
                    + 0.000952891492498337 * aoa2 * mach1
                    + -0.03713084002224525 * aoa1 * mach2
                    + -8.738531480695034 * mach3
                    + -5.300459069625812e-07 * aoa4
                    + -3.860833826539002e-05 * aoa3 * mach1
                    + 0.00012197092880850173 * aoa2 * mach2
                    + 0.02169048598127654 * aoa1 * mach3
                    + 3.8381431941150304 * mach4;

            }
            else if (mach1 <= 2.5)
            {
                C_d = -3.7345464157352875
                    + -0.021181219135558413 * aoa1
                    + 9.920282009469302 * mach1
                    + 0.0012893340153573994 * aoa2
                    + 0.04244683350949655 * aoa1 * mach1
                    + -9.138041801129381 * mach2
                    + -8.624742765568063e-06 * aoa3
                    + -0.00018539594817467342 * aoa2 * mach1
                    + -0.028455098725651162 * aoa1 * mach2
                    + 3.605595006014011 * mach3
                    + -3.3432488691018114e-07 * aoa4
                    + 9.577832765508875e-06 * aoa3 * mach1
                    + -6.0242459194089406e-05 * aoa2 * mach2
                    + 0.005770291051328308 * aoa1 * mach3
                    + -0.5163373717766221 * mach4;


            }
            else if (mach1 <= 5.0)
            {
                C_d = 0.09621766511560526
                    + 0.0028871402429949946 * aoa1
                    + 0.24720296689394455 * mach1
                    + 0.0006230823799741877 * aoa2
                    + 0.0039949754389784825 * aoa1 * mach1
                    + -0.22564130237384813 * mach2
                    + -2.1646270884879146e-05 * aoa3
                    + 0.0003643962897843541 * aoa2 * mach1
                    + -0.004539907427730426 * aoa1 * mach2
                    + 0.06486533400540355 * mach3
                    + 7.376554473585012e-08 * aoa4
                    + 4.559704070408374e-06 * aoa3 * mach1
                    + -8.720151109828812e-05 * aoa2 * mach2
                    + 0.0007397811034185737 * aoa1 * mach3
                    + -0.005948980070235025 * mach4;

            }
            else
            {
                C_d = 0.2500347212461588
                    + 0.0005771604947031019 * aoa1
                    + -0.07217789096250898 * mach1
                    + 0.00047744188688542366 * aoa2
                    + -0.0012706945215407868 * aoa1 * mach1
                    + 0.010245300592043663 * mach2
                    + 7.308474022131236e-06 * aoa3
                    + -7.0855321864752e-06 * aoa2 * mach1
                    + 9.180400845612842e-05 * aoa1 * mach2
                    + -0.0005924894636624126 * mach3
                    + -1.0117509522138048e-07 * aoa4
                    + 1.707709757023813e-07 * aoa3 * mach1
                    + -1.4762855095269744e-07 * aoa2 * mach2
                    + -1.8986152911030522e-06 * aoa1 * mach3
                    + 1.1978824308260162e-05 * mach4;


            }

            /*
            correction to account for control surfaces and the oms pods
            let FAR predict the final Cl and Cd, extracted the deviations, and fitter with a 2 degree polynomial
             */

            
            C_d += 0.79 * (-0.015881489614976013
                + -0.0003981020409242033 * aoa1
                + -0.001160689364231904 * mach1
                + -9.465932645687637e-05 * aoa2
                + 5.7595868868018406e-05 * aoa1 * mach1
                + 9.365456744424538e-06 * mach2);

            //C_d += (-0.014578839156044639 + -0.003278987927832168 * aoa1 + 6.907416309993462e-05 * mach1);

            return C_d;
        }

    }
}
