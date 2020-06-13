﻿using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace DarkIntentionsWoohoo
{
    class BetterBedFinder
    {
        // Token: 0x06001607 RID: 5639 RVA: 0x000ADC84 File Offset: 0x000AC084
        public static Building_Bed DoBetterBedFinder(Pawn pawn, Pawn mate)
        {
            //--Log.Message("Looking for a bed", false);
            if (pawn == null || mate == null) return null;

            Building_Bed building_Bed;
            if ((building_Bed = PawnBedBigEnough(pawn)) != null)
            {
                //--Log.Message("I got a big bed", false);
                return building_Bed;
            }
            else if ((building_Bed = PawnBedBigEnough(pawn)) != null)
            {
                //--Log.Message("They got a big bed", false);
                return building_Bed;
            }
            else
            {
                var allBeds = pawn.Map.listerBuildings.allBuildingsColonist
                    .ConvertAll(x => x as Building_Bed);

                if (!allBeds.Any())
                {
                    //--Log.Message("There are no beds...", false);
                    return null;
                }

                IEnumerable<Building_Bed> bigBeds = allBeds
                        .Where(x => x != null && x.SleepingSlotsCount > 1 && !x.Medical).ToList()
                    ;

                if (!bigBeds.Any())
                {
                    //--Log.Message("No big beds anywhere, great.", false);
                    return null;
                }

                var priority = bigBeds.Where(x => x.OwnersForReading.Contains(pawn) || x.OwnersForReading.Contains(mate));
                var buildingBeds = priority.ToList();
                if (buildingBeds.Any())
                {
                    //--Log.Message("Looks like We own beds, lets check those.", false);
                    foreach (Building_Bed bed in buildingBeds)
                    {
                        if (bed != null)
                        {
                            return bed;
                        }
                        else
                        {
                            Log.Error("How the hell is the owned bed null?", true);
                        }
                    }
                }
                else
                {
                    //--Log.Message("Maybe the colony has some empty ones...", false);
                }

                //not else incase that fails for some reason, shouldnt but lets let the logic flow
                ///What we know, All Beds with at least 2 slots do not belong to our couple.
                //
                // We will now look at the the available beds and look for an empty one
                // if not well woohoo 
                foreach (Building_Bed openbed in bigBeds.Where(x => x.OwnersForReading == null || !x.OwnersForReading.Any()))
                {
                    if(canReserve(pawn, openbed) && canReserve(mate, openbed))
                    //--Log.Message("Found us a place to woohoo", false);
                    return openbed;
                }

                //lets steal a bed!
                foreach (Building_Bed stolenBed in bigBeds.Where(bed =>
                    bed.CurOccupants == null || !bed.CurOccupants.Any()))
                {
                    //-- /* Log.Message("Stealing a bed...", false); */
                    //Log.Message("Stealing Bed!");
                    if(canReserve(pawn, stolenBed) && canReserve(mate, stolenBed))
                    return stolenBed;
                }
            }


            //--Log.Message("Nope, no beds", false);

            return null;
        }

        private static bool canReserve(Pawn traveler, Building_Bed building_Bed)
        {
            LocalTargetInfo target = building_Bed;
            PathEndMode peMode = PathEndMode.OnCell;
            Danger maxDanger = Danger.Some;
            int sleepingSlotsCount = building_Bed.SleepingSlotsCount;
            if (!traveler.CanReserveAndReach(target, peMode, maxDanger, sleepingSlotsCount, -1, null, false))
            {
                return false;
            }

            return true;
        }

        private static Building_Bed PawnBedBigEnough(Pawn pawn)
        {
            if (pawn != null)
            {
                Building_Bed building_Bed = pawn.CurrentBed();

                if (building_Bed != null && building_Bed.SleepingSlotsCount > 1)
                {
                    //Score// Woohoo here.
                    return building_Bed;
                }
            }

            return null;
        }
    }
}