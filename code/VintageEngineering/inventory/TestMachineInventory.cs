﻿using System;
using System.Collections.Generic;
using Vintagestory.API;
using Vintagestory.Common;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.API.Client;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using VintageEngineering.RecipeSystem.Recipes;
using VintageEngineering.RecipeSystem;

namespace VintageEngineering
{
    public class TestMachineInventory : InventoryBase, ISlotProvider
    {
        ICoreClientAPI capi;
        ICoreServerAPI sapi;
        private ItemSlot[] _slots;
        public IPlayer machineuser;

        public override float GetSuitability(ItemSlot sourceSlot, ItemSlot targetSlot, bool isMerge)
        {
            if (targetSlot == _slots[_slots.Length - 1])
            {
                return 0f;
            }
            if (targetSlot == _slots[0])
            {
                return 4f;
            }
            return base.GetSuitability(sourceSlot, targetSlot, isMerge);
        }

        /// <summary>
        /// Can the sinkSlot contain the item is sourceSlot?
        /// </summary>
        /// <param name="sinkSlot"></param>
        /// <param name="sourceSlot"></param>
        /// <returns>True if yes</returns>
        public override bool CanContain(ItemSlot sinkSlot, ItemSlot sourceSlot)
        {
            // strict slot restrictions
            if (GetSlotId(sinkSlot) == 0)
            {
                // input slot
                return true;
            }
            else if (GetSlotId(sinkSlot) == 3)
            {
                // Mold slot
                string moldcode = sourceSlot.Itemstack?.Collectible?.FirstCodePart();
                return (moldcode == "vepressmold");
            }
            return true;
        }

        public override bool HasOpened(IPlayer player)
        {
            return (machineuser != null && machineuser.PlayerUID == player.PlayerUID);
        }

        public override bool RemoveOnClose { get { return true; } }

        public ItemSlot[] Slots
        {
            get { return this._slots; }
        }

        public override int Count
        {
            get { return _slots.Length; }
        }

        public override ItemSlot this[int slotId]
        {
            get
            {
                if (slotId > 3 || slotId < 0) return null;

                return _slots[slotId];
            }
            set
            {
                if (slotId > 3 || slotId < 0) throw new ArgumentOutOfRangeException("slotId");
                if (value == null) throw new ArgumentNullException("value");
                _slots[slotId] = value;
            }
        }

        protected override ItemSlot NewSlot(int i)
        {
            return new ItemSlotSurvival(this);
        }

        /// <summary>
        /// Slot index 0 is input, 1 & 2 is output, and 3 is press mold
        /// </summary>
        /// <param name="inventoryID"></param>
        /// <param name="api"></param>
        public TestMachineInventory(string inventoryID, ICoreAPI api) : base(inventoryID, api)
        {
            _slots = base.GenEmptySlots(4);
            _slots[3].MaxSlotStackSize = 1;
            _slots[3].StorageType = EnumItemStorageFlags.Custom2;
        }

        public override void LateInitialize(string inventoryID, ICoreAPI api)
        {
            base.LateInitialize(inventoryID, api);
            if (api.Side == EnumAppSide.Server)
            {
                sapi = api as ICoreServerAPI;
            }
            else
            {
                capi = api as ICoreClientAPI;
            }

        }

        public override ItemSlot GetAutoPushIntoSlot(BlockFacing atBlockFace, ItemSlot fromSlot)
        {
            return _slots[0];
        }

        public override ItemSlot GetAutoPullFromSlot(BlockFacing atBlockFace)
        {

            if (!_slots[1].Empty) return _slots[1]; // empty the first output slot first, then the second, extra output

            return _slots[2];
        }

        public override void FromTreeAttributes(ITreeAttribute tree)
        {
            this._slots = this.SlotsFromTreeAttributes(tree, this._slots, null);
        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            base.SlotsToTreeAttributes(_slots, tree);
            this.ResolveBlocksOrItems();
        }
    }
}
