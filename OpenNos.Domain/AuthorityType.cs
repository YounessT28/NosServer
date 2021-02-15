﻿/*
 * This file is part of the OpenNos Emulator Project. See AUTHORS file for Copyright information
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 */

namespace OpenNos.Domain
{
    public enum AuthorityType : short
    {
        //Unconfirmed = -1 > Will be done in the future
        User = 0,
        GameSage = 1,
        GameMaster = 2,
        Administrator = 3,
        Banned = 100 //Unreachable, so it doesn't matter if it's higher than Administrator

    }
}