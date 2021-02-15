/*
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

using System;


namespace OpenNos.Data
{
    [Serializable]
    public class ActPartDTO : SynchronizableBaseDTO
    {
        #region Properties

        public byte ActPartId { get; set; }

        public byte ActPartNumber { get; set; }

        public byte ActId { get; set; }

        public ActDTO Act { get; set; }

        public byte MaxTs { get; set; }

        public CharacterActPartDTO CharacterActParts { get; set; }

        #endregion
    }
}