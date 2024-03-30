﻿using VintageEngineering.Electrical;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace VintageEngineering
{
    public class BlockKiln : ElectricBlock
    {
        ICoreClientAPI capi;
        ICoreServerAPI sapi;
        public override void OnLoaded(ICoreAPI api)
        {
            base.OnLoaded(api);
            if (api.Side == EnumAppSide.Server)
            {
                sapi = api as ICoreServerAPI;
            }
            else
            {
                capi = api as ICoreClientAPI;
            }
        }
        public override string GetPlacedBlockInfo(IWorldAccessor world, BlockPos pos, IPlayer forPlayer)
        {
            BEKiln beMach = world.BlockAccessor.GetBlockEntity(pos) as BEKiln;
            if (beMach != null)
            {
                return beMach.GetOutputText() + base.GetPlacedBlockInfo(world, pos, forPlayer);
            }
            else
            {
                return base.GetPlacedBlockInfo(world, pos, forPlayer);
            }
        }
    }
}
