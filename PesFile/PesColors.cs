/*
Embroidery Reader - an application to view .pes embroidery designs

Copyright (C) 2016 Nathan Crawford
 
This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
 
You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA
02111-1307, USA.

A copy of the full GPL 2 license can be found in the docs directory.
You can contact me at http://www.njcrawford.com/contact/.
*/

namespace PesFile
{
    public class PesColors
    {
        public static int[,] colorMap = new int[,] {
            //red  grn  blu
    	    {  0,   0,   0}, // 0 Not used
    	    { 14,  31, 124}, // 1 
    	    { 10,  85, 163}, // 2
    	    { 48, 135, 119}, // 3
    	    { 75, 107, 175}, // 4
    	    {237,  23,  31}, // 5
    	    {209,  92,   0}, // 6
    	    {145,  54, 151}, // 7
    	    {228, 154, 203}, // 8
    	    {145,  95, 172}, // 9
    	    {157, 214, 125}, // 10
    	    {232, 169,   0}, // 11
    	    {254, 186,  53}, // 12
    	    {255, 255,   0}, // 13
    	    {112, 188,  31}, // 14
    	    {186, 152,   0}, // 15
    	    {168, 168, 168}, // 16
    	    {123, 111,   0}, // 17
    	    {255, 255, 179}, // 18
    	    { 79,  85,  86}, // 19
    	    {  0,   0,   0}, // 20
    	    { 11,  61, 145}, // 21
    	    {119,   1, 118}, // 22
    	    { 41,  49,  51}, // 23
    	    { 42,  19,   1}, // 24
    	    {246,  74, 138}, // 25
    	    {178, 118,  36}, // 26
    	    {252, 187, 196}, // 27
    	    {254,  55,  15}, // 28
    	    {240, 240, 240}, // 29
    	    {106,  28, 138}, // 30
    	    {168, 221, 196}, // 31
    	    { 37, 132, 187}, // 32
    	    {254, 179,  67}, // 33
    	    {255, 240, 141}, // 34
    	    {208, 166,  96}, // 35
    	    {209,  84,   0}, // 36
    	    {102, 186,  73}, // 37
    	    { 19,  74,  70}, // 38
    	    {135, 135, 135}, // 39
    	    {216, 202, 198}, // 40
    	    { 67,  86,   7}, // 41
    	    {254, 227, 197}, // 42
    	    {249, 147, 188}, // 43
    	    {  0,  56,  34}, // 44
    	    {178, 175, 212}, // 45
    	    {104, 106, 176}, // 46
    	    {239, 227, 185}, // 47
    	    {247,  56, 102}, // 48
    	    {181,  76, 100}, // 49
    	    { 19,  43,  26}, // 50
    	    {199,   1,  85}, // 51
    	    {254, 158,  50}, // 52
    	    {168, 222, 235}, // 53
    	    {  0, 103,  26}, // 54
    	    { 78,  41, 144}, // 55
    	    { 47, 126,  32}, // 56
    	    {253, 217, 222}, // 57
    	    {255, 217,  17}, // 58
    	    {  9,  91, 166}, // 59
    	    {240, 249, 112}, // 60
    	    {227, 243,  91}, // 61
    	    {255, 200, 100}, // 62
    	    {255, 200, 150}, // 63
    	    {255, 200, 200} // 64
        };
    }
}
