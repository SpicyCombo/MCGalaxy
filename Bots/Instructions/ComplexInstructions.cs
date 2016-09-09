﻿/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.opensource.org/licenses/ecl2.php
    http://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;

namespace MCGalaxy.Bots {
    
    /// <summary> Causes the bot to move towards the closest human, within a defined search radius. </summary>
    public sealed class HuntInstruction : BotInstruction {
        public override string Name { get { return "Hunt"; } }
        
        const int searchRadius = 75;
        public override bool Execute(PlayerBot bot, InstructionData data) {
            int dist = searchRadius * 32;
            Player[] players = PlayerInfo.Online.Items;
            Player closest = null;
            
            foreach (Player p in players) {
                if (p.level != bot.level || p.invincible) continue;
                
                int dx = p.pos[0] - bot.pos[0], dy = p.pos[1] - bot.pos[1], dz = p.pos[2] - bot.pos[2];
                int curDist = Math.Abs(dx) + Math.Abs(dy) + Math.Abs(dz);
                if (curDist >= dist) continue;
                closest = p;
            }
            
            if (closest == null) return true;
            MoveTowards(bot, closest);
            return false;
        }
        
        static void MoveTowards(PlayerBot bot, Player p) {
            int dx = p.pos[0] - bot.pos[0], dy = p.pos[1] - bot.pos[1], dz = p.pos[2] - bot.pos[2];
            bot.foundPos = p.pos;
            bot.movement = true;
            
            Vec3F32 dir = new Vec3F32(dx, dy, dz);
            dir = Vec3F32.Normalise(dir);
            byte yaw, pitch;
            DirUtils.GetYawPitch(dir, out yaw, out pitch);
            
            // If we are very close to a player, switch from trying to look
            // at them to just facing the opposite direction to them
            if (Math.Abs(dx) >= 4 || Math.Abs(dz) >= 4) {
                bot.rot[0] = yaw;
            } else if (p.rot[0] < 128) {
                bot.rot[0] = (byte)(p.rot[0] + 128);
            } else {
                bot.rot[0] = (byte)(p.rot[0] - 128);
            }
            bot.rot[1] = pitch;
        }
        
        public override InstructionData Parse(string[] args) {
            return default(InstructionData);
        }
    }
    
    /// <summary> Causes the bot to kill nearby humans. </summary>
    public sealed class KillInstruction : BotInstruction {
        public override string Name { get { return "Kill"; } }

        public override bool Execute(PlayerBot bot, InstructionData data) {
            Player[] players = PlayerInfo.Online.Items;            
            foreach (Player p in players) {
                int dx = Math.Abs(bot.pos[0] - p.pos[0]);
                int dy = Math.Abs(bot.pos[1] - p.pos[1]);
                int dz = Math.Abs(bot.pos[2] - p.pos[2]);
                
                if (dx <= 8 && dy <= 16 && dz <= 8) {
                    p.HandleDeath(Block.Zero);
                }
            }
            return true;
        }
        
        public override InstructionData Parse(string[] args) {
            return default(InstructionData);
        }
    }
}
