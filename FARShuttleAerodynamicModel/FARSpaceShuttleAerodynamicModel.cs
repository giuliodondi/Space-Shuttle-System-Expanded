

using System;
using FerramAerospaceResearch;

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

        private double maxOverridableAoa = 50;

        private double minOverridableAoa = -10;


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
            if (minOverridableAoa <= AoA && AoA <= maxOverridableAoa) {
                Cl = getSpaceShuttleCL(MachNumber, AoA);
                Cd = getSpaceShuttleCd(MachNumber, AoA);
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

            //double shuttleLD = Cl / Cd;
            //UnityEngine.Debug.Log(String.Format("Shuttle Mach, Aoa, Cl, Cd, LD :  {0}, {1}, {2}, {3}, {4}", MachNumber, AoA * radiansToDegrees, Cl, Cd, shuttleLD));

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

            C_d += 0.8 * (-0.015881489614976013
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
