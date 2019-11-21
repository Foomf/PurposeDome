﻿// Copyright (c) 2019 Foomf
//
// This software is provided 'as-is', without any express or implied
// warranty. In no event will the authors be held liable for any damages
// arising from the use of this software.
//
// Permission is granted to anyone to use this software for any purpose,
// including commercial applications, and to alter it and redistribute it
// freely, subject to the following restrictions:
//
// 1. The origin of this software must not be misrepresented; you must not
//    claim that you wrote the original software. If you use this software
//    in a product, an acknowledgment in the product documentation would be
//    appreciated but is not required.
// 2. Altered source versions must be plainly marked as such, and must not be
//    misrepresented as being the original software.
// 3. This notice may not be removed or altered from any source distribution.

namespace Blyss.PurposeDome.Space
{
    using System;

    public class Map
    {
        private readonly int _width;
        private readonly int _height;
        private readonly Cell[][] _cells;

        public Map(int width, int height)
        {
            _width = width;
            _height = height;

            _cells = new Cell[width][];
            for (var x = 0; x < width; ++x)
            {
                _cells[width] = new Cell[height];

                for (var y = 0; y < height; ++y)
                {
                    _cells[x][y] = new Cell();
                }
            }
        }

        public Cell CellAt(int x, int y)
        {
            if (x < 0 || x >= _width)
            {
                throw new ArgumentOutOfRangeException(nameof(x));
            }

            if (y < 0 || y >= _height)
            {
                throw new ArgumentOutOfRangeException(nameof(y));
            }

            return _cells[x][y];
        }

    }
}
