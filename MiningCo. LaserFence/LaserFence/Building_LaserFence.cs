﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;   // Always needed
using RimWorld;      // RimWorld specific functions are found here
using Verse;         // RimWorld universal objects are here
//using Verse.AI;      // Needed when you do something with the AI
//using RimWorld.SquadAI;
//using Verse.Sound; // Needed when you do something with the Sound

namespace LaserFence
{
    /// <summary>
    /// Building_LaserFence class.
    /// </summary>
    /// <author>Rikiki</author>
    /// <permission>Use this code as you want, just remember to add a link to the corresponding Ludeon forum mod release thread.
    /// Remember learning is always better than just copy/paste...</permission>
    public class Building_LaserFence : Building
    {
        public const int buildingCheckPeriodInTicks = 30;
        public const int plantCheckPeriodInTick = GenTicks.TickRareInterval;
        public int nextBuildingCheckTick = 0;
        public int nextPlantCheckTick = 0;
        public Building_LaserFencePylon pylon = null;

        // ===================== Setup work =====================
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            this.nextBuildingCheckTick = Find.TickManager.TicksGame + Rand.Range(0, buildingCheckPeriodInTicks);
            this.nextPlantCheckTick = Find.TickManager.TicksGame + Rand.Range(0, plantCheckPeriodInTick);
        }


        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look<Building_LaserFencePylon>(ref pylon, "pylon");
        }

        // ===================== Main function =====================
        public override void Tick()
        {
            // Check if a new building is cutting the laser fence.
            if (Find.TickManager.TicksGame >= this.nextBuildingCheckTick)
            {
                this.nextBuildingCheckTick = Find.TickManager.TicksGame + buildingCheckPeriodInTicks;
                if (this.Position.GetEdifice(this.Map) != null)
                {
                    if (pylon != null)
                    {
                        pylon.Notify_EdificeIsBlocking();
                    }
                }
            }
            // Check if a plant or pawn is in the laser fence path.
            if (Find.TickManager.TicksGame >= this.nextPlantCheckTick)
            {
                this.nextPlantCheckTick = Find.TickManager.TicksGame + plantCheckPeriodInTick;
                List<Thing> thingList = this.Position.GetThingList(this.Map);
                for (int thingIndex = thingList.Count - 1; thingIndex >= 0; thingIndex--)
                {
                    Thing thing = thingList[thingIndex];
                    if (thing is Plant)
                    {
                        FireUtility.TryStartFireIn(this.Position, this.Map, 0.1f);
                        break;
                    }
                    if (thing is Pawn)
                    {
                        FireUtility.TryAttachFire(thing, 0.1f);
                        break;
                    }
                }
            }
        }
    }
}
