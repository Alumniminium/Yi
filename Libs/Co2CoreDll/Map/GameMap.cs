using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CO2_CORE_DLL.Map
{
    public unsafe class GameMap
    {
        public const Int32 MAX_PATH = 260;

        private const Int32 LAYER_NONE = 0;
        private const Int32 LAYER_TERRAIN = 1;
        private const Int32 LAYER_FLOOR = 2;
        private const Int32 LAYER_INTERACTIVE = 3;
        private const Int32 LAYER_SCENE = 4;
        private const Int32 LAYER_SKY = 5;
        private const Int32 LAYER_SURFACE = 6;

        private const Int32 MAP_NONE = 0;
        private const Int32 MAP_TERRAIN = 1;
        private const Int32 MAP_TERRAIN_PART = 2;
        private const Int32 MAP_SCENE = 3;
        private const Int32 MAP_COVER = 4;
        private const Int32 MAP_ROLE = 5;
        private const Int32 MAP_HERO = 6;
        private const Int32 MAP_PLAYER = 7;
        private const Int32 MAP_PUZZLE = 8;
        private const Int32 MAP_3DSIMPLE = 9;
        private const Int32 MAP_3DEFFECT = 10;
        private const Int32 MAP_2DITEM = 11;
        private const Int32 MAP_3DNPC = 12;
        private const Int32 MAP_3DOBJ = 13;
        private const Int32 MAP_3DTRACE = 14;
        private const Int32 MAP_SOUND = 15;
        private const Int32 MAP_2DREGION = 16;
        private const Int32 MAP_3DMAGICMAPITEM = 17;
        private const Int32 MAP_3DITEM = 18;
        private const Int32 MAP_3DEFFECTNEW = 19;

        private MySize MapSize;
        private Cell[,] Cells; //A Cell constains only useful data.
        private Dictionary<Int32, PassageInfo> Passages;
        private List<RegionInfo> Regions;

        private String Folder;

        public GameMap(String Path)
        {
            MapSize = new MySize();
            Cells = new Cell[0, 0];
            Passages = new Dictionary<Int32, PassageInfo>();
            Regions = new List<RegionInfo>();

            Folder = System.IO.Path.GetDirectoryName(Path) + "\\..\\..\\";
            LoadDataMap(Path);
        }

        public Int32 Width { get { return MapSize.Width; } }
        public Int32 Height { get { return MapSize.Height; } }

        public Cell? GetCell(Int32 X, Int32 Y)
        {
            if (X < 1 || Y < 1)
                return null;

            if (X > MapSize.Width || Y > MapSize.Height)
                return null;

            return Cells[X - 1, Y - 1];
        }

        private void LoadDataMap(String Path)
        {
            if (String.IsNullOrEmpty(Path))
                throw new ArgumentNullException();

            //Load the DMap file...
            using (var Stream = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                UInt32 Version; //Currently 1004?
                UInt32 Data;

                Stream.Read(&Version, sizeof(UInt32));
                Stream.Read(&Data, sizeof(UInt32));
                //Console.WriteLine("Version: {0} Data: {1}", Version, Data);

                Byte* pFileName = stackalloc Byte[MAX_PATH];
                Stream.Read(pFileName, MAX_PATH);

                var FullFileName = Folder + new String((SByte*)pFileName);
                //LoadPuzzle(FullFileName);
                //Console.WriteLine("Puzzle: {0} [{1}]", FullFileName, MAX_PATH);

                fixed (MySize* pMapSize = &MapSize)
                    Stream.Read(pMapSize, sizeof(MySize));
                //Console.WriteLine("Width = {0}, Height = {1}", MapSize.Width, MapSize.Height);

                Cells = new Cell[MapSize.Width, MapSize.Height];
                for (var i = 0; i < MapSize.Height; i++)
                {
                    UInt32 CheckData = 0;
                    for (var j = 0; j < MapSize.Width; j++)
                    {
                        var Layer = new LayerInfo();

                        Stream.Read(&Layer.Mask, sizeof(UInt16));
                        Stream.Read(&Layer.Terrain, sizeof(UInt16));
                        Stream.Read(&Layer.Altitude, sizeof(Int16));
                        CheckData += (UInt32)((Layer.Mask * (Layer.Terrain + i + 1)) +
                                              ((Layer.Altitude + 2) * (j + 1 + Layer.Terrain)));

                        //Console.WriteLine("Cell({0}, {1}) MASK[{2}] TERRAIN[{3}] ALTITUDE[{4}]", j, i, Layer.Mask, Layer.Terrain, Layer.Altitude);
                        Cells[j, i] = new Cell(Layer);
                    }
                    UInt32 MapCheckData;
                    Stream.Read(&MapCheckData, sizeof(UInt32));

                    if (MapCheckData != CheckData)
                        throw new Exception("Map checksum failed!");
                }

                LoadDataPassage(Stream);
                /*if (Version == 1003)
                    LoadDataRegion(Stream);*/
                LoadDataLayer(Stream);

                //The rest are LAYER_SCENE, but useless for a server. I'll not implement the rest as it would only
                //slow down the loading.
            }  
        }

        private void LoadDataPassage(FileStream Stream)
        {
            if (Stream == null)
                throw new ArgumentNullException();

            Int32 Amount;
            Stream.Read(&Amount, sizeof(Int32));

            for (var i = 0; i < Amount; i++)
            {
                PassageInfo Passage;
                Stream.Read(&Passage, sizeof(PassageInfo));

                //Console.WriteLine("Passage[{0}] ({1}, {2}", Passage.Index, Passage.PosX, Passage.PosY);
                Passages.Add(Passage.Index, Passage);
            }
        }

        private void LoadDataRegion(FileStream Stream)
        {
            if (Stream == null)
                throw new ArgumentNullException();

            Int32 Amount;
            Stream.Read(&Amount, sizeof(Int32));

            for (var i = 0; i < Amount; i++)
            {
                RegionInfo Region;
                Stream.Read(&Region, sizeof(RegionInfo));
                Regions.Add(Region);
            }
        }

        private void LoadDataLayer(FileStream Stream)
        {
            if (Stream == null)
                throw new ArgumentNullException();

            Int32 Amount;
            Stream.Read(&Amount, sizeof(Int32));

            for (var i = 0; i < Amount; i++)
            {
                Int32 MapObjType;
                Stream.Read(&MapObjType, sizeof(Int32));

                switch (MapObjType)
                {
                    case MAP_COVER:
                        {
                            //Do nothing with it...
                            Stream.Seek(sizeof(C2DMapCoverObj), SeekOrigin.Current);
                            break;
                        }
                    case MAP_TERRAIN:
                        {
                            var MapObj = new C2DMapTerrainObj();
                            Stream.Read(MapObj.FileName, 260);
                            Stream.Read(&MapObj.PosCell, sizeof(MyPos));
                            MapObj.Load(Folder);

                            //The server only need the new LayerInfo, so it will be merged
                            //and the object will be deleted
                            foreach (var Part in MapObj.Parts)
                            {
                                for (var j = 0; j < Part.SizeBase.Width; j++)
                                {
                                    for (var k = 0; k < Part.SizeBase.Height; k++)
                                    {
                                        var X = ((MapObj.PosCell.X + Part.PosSceneOffset.X) + j) - Part.SizeBase.Width;
                                        var Y = ((MapObj.PosCell.Y + Part.PosSceneOffset.Y) + k) - Part.SizeBase.Height;
                                        Cells[X, Y] = new Cell(Part.Cells[j, k]);
                                    }
                                }
                            }
                            break;
                        }
                    case MAP_SOUND:
                        {
                            //Do nothing with it...
                            Stream.Seek(sizeof(CMapSound), SeekOrigin.Current);
                            break;
                        }
                    case MAP_3DEFFECT:
                        {
                            //Do nothing with it...
                            Stream.Seek(sizeof(C3DMapEffect), SeekOrigin.Current);
                            break;
                        }
                    case MAP_3DEFFECTNEW:
                        {
                            //Do nothing with it...
                            Stream.Seek(sizeof(C3DMapEffectNew), SeekOrigin.Current);
                            break;
                        }
                    default:
                        break;
                }
            }
        }
    }
}
