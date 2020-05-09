using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using WindowsFormsApp1.GeometryModel;
using WindowsFormsApp1.Utilities;
using Newtonsoft.Json;
using System.Runtime.Serialization;

//still todos
//TODO: branchelés - model operáció sorrend invariáns? akkor lehet őket osszevonogatni
//TODO: a hurok esetben nem kapunk jó eredményt
//TODO: make tests and handle failures - find step that fails and correct there


//felismerések:
//csak UI szimuláció futtatás során jön elő, ha breakpointok vannak, nem
//a szoba szétesés továbbra is probléma
//hiába avan benne, hogy move esetén ha van ferde vonal, dobja el, akkor is végrehajta a mozgatást
//a szoba eldobás probléma jó eséllyel azt mutatja, hogy nem adódik hozzá a megfelelő szoba
//TODO: bejárhatóság mint split, legyen egy valid lépés opció
//TODO: handle openings
//TODO: közlekedő felvétele - szobatípus

//done
//conditional breakpoint at 22. simulation step before move
//volt egy darab pont duplikálva - még mindig. - lehet, hogy több vonal van a rendszerben
//switch nem működik jól
//kézi léptetést engedjen - intuitívabb lenne a kolstegfüggvény tervezése
//implementláni kell az új bool propertyt
//a listában kijelölt vonalnak látszódnia kéne az UI-on
//a léptetés negatív irányban nem működik jól
//közvetlen következő teendő az, hogy a mozgatás és a split kiírja a költségeket is
//kérdés, hogy hol történjen a mozgatás, most ugye az UI részen van
//költség: belső fal (több mint 1 fajta relatedroom)
//helyiségarány - befoglaló téglalappal
//kell-e az observablecollection - nem kell az odavissza hatás
//load model
//save model
//körüljárás alapján lehet megmondani, hogy melyik szobába kerüljün
//körüljárás jó a kirajzoláshoz és a karbantartáshoz is
//le kell kezelni minden módosítás során a szobák állapotváltozásait, ha nem jó a lépés, dobjuk el
//a lépés előtt lehetne tárolni az előző állapotot
//implement deepcopy for model class
//hogyan változik a loss
namespace WindowsFormsApp1 {
    public class Model {
        [JsonIgnore]
        [IgnoreDataMember]
        Random rand = new Random(10); //this random integer ensures that the simulation keeps the same
        /// <summary>
        /// nullable constructor
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="rooms"></param>
        public Model(List<MyLine> lines = null, List<MyRoom> rooms = null) {
            if (lines != null) {
                modelLines = new ObservableCollection<MyLine>(lines);
            }
            if (rooms != null) {
                modelRooms = new ObservableCollection<MyRoom>(rooms);
            }
        }
        //TODO: set this in the move/split/switch 
        
        public bool IsInInvalidState { get; set; } //when a move makes the model invalid, we set this switch
        public string SaveStateToString() {
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            return jsonString;
        }
        public void LoadStateFromString(string jsonString) {
            Model obj = Newtonsoft.Json.JsonConvert.DeserializeObject<Model>(jsonString);
            modelLines = obj.modelLines;
            modelRooms = obj.modelRooms;
        }
        [JsonIgnore] 
        [IgnoreDataMember] 
        public ModelType loadedModelType { get; set; }
        public ObservableCollection<MyLine> modelLines { get; set; } = new ObservableCollection<MyLine>();
        public ObservableCollection<MyRoom> modelRooms { get; set; } = new ObservableCollection<MyRoom>();

        [JsonIgnore]
        [IgnoreDataMember]
        /// <summary>
        /// Returns the points of the model ordered by guid
        /// </summary>
        public List<MyPoint> ModelPoints {
            get {
                List<MyPoint> starts = modelLines.Select(i => i.StartMyPoint).ToList();
                List<MyPoint> ends = modelLines.Select(i => i.EndMyPoint).ToList();
                starts.AddRange(ends);
                starts = starts.OrderBy(x => x.Guid).ToList();
                return starts;
            }
        }

        public List<MyPoint> GetUniqueModelPoints() {
            List<MyPoint> starts = ModelPoints;
            //int nStep = 2;
            //if (starts.Count % 2 != 0) {
            //    throw new Exception("Model points should be doubled");
            //}
            //for (var i = 0; i < starts.Count; i += 2) {
            //    MyPoint myPoint1 = starts[i];
            //    MyPoint myPoint2 = starts[i + 1];
            //    if (myPoint1.Guid != myPoint2.Guid) {
            //        throw new Exception("Invalid model points");
            //    }
            //}
            //starts = starts.Where((x, i) => i % nStep == 0).ToList();
            starts = starts.Distinct().ToList();
            return starts;
        }




        public void InitSimplestModel() {
            loadedModelType = ModelType.Simplest;
            modelLines = new ObservableCollection<MyLine>();
            modelRooms = new ObservableCollection<MyRoom>();
            MyPoint p1 = new MyPoint(100, 100);
            MyPoint p2 = new MyPoint(400, 100);
            MyPoint p3 = new MyPoint(400, 400);
            MyPoint p4 = new MyPoint(100, 400);
            MyLine line1 = new MyLine(p1, p2);
            modelLines.Add(line1);
            MyLine line2 = new MyLine(p2, p3);
            modelLines.Add(line2);
            MyLine line3 = new MyLine(p3, p4);
            modelLines.Add(line3);
            MyLine line4 = new MyLine(p4, p1);
            modelLines.Add(line4);

            MyRoom first = new MyRoom("FirstRoom", "1", RoomType.Kitchen);

            foreach (MyLine modelLine in new List<MyLine>() { line1, line2, line3, line4 }) {
                modelLine.relatedRooms.Add(first);
            }
            CalculateAllRooms();
            Logger.WriteLog("InitSimplestModel() finished");
        }
        public void InitSimpleModel() {
            loadedModelType = ModelType.Simple;

            modelLines = new ObservableCollection<MyLine>();
            modelRooms = new ObservableCollection<MyRoom>();
            MyPoint q1 = new MyPoint(100, 0);
            MyPoint q2 = new MyPoint(400, 0);
            MyPoint p1 = new MyPoint(100, 100);
            MyPoint p2 = new MyPoint(400, 100);
            MyPoint p3 = new MyPoint(400, 400);
            MyPoint p4 = new MyPoint(100, 400);
            MyLine line1 = new MyLine(p1, p2);
            modelLines.Add(line1);
            MyLine line2 = new MyLine(p2, p3);
            modelLines.Add(line2);
            MyLine line3 = new MyLine(p3, p4);
            modelLines.Add(line3);
            MyLine line4 = new MyLine(p4, p1);
            modelLines.Add(line4);
            MyLine l1 = new MyLine(q1, p1);
            MyLine l2 = new MyLine(q2, p2);
            MyLine l3 = new MyLine(q1, q2);
            modelLines.Add(l1);
            modelLines.Add(l2);
            modelLines.Add(l3);

            MyRoom first = new MyRoom("FirstRoom", "1", RoomType.Kitchen);
            MyRoom second = new MyRoom("SecondRoom", "2", RoomType.LivingRoom);

            foreach (MyLine modelLine in new List<MyLine>() { line1, line2, line3, line4 }) {
                modelLine.relatedRooms.Add(first);
            }

            foreach (MyLine modelLine in new List<MyLine>() { l1, l2, l3, line1 }) {
                modelLine.relatedRooms.Add(second);
            }

            CalculateAllRooms();
            Logger.WriteLog("InitSimpleModel() finished");
        }
        public void InitNormalModel() {
            loadedModelType = ModelType.Normal;
            modelLines = new ObservableCollection<MyLine>();
            modelRooms = new ObservableCollection<MyRoom>();
            MyPoint a1 = new MyPoint(0, 0);
            MyPoint a2 = new MyPoint(200, 0);
            MyPoint a3 = new MyPoint(200, 200);
            MyPoint a4 = new MyPoint(0, 200);

            MyPoint a5 = new MyPoint(200, 400);
            MyPoint a6 = new MyPoint(0, 400);
            MyPoint a7 = new MyPoint(400, 0);
            MyPoint a8 = new MyPoint(400, 200);
            MyPoint a9 = new MyPoint(400, 400);

            MyLine l12 = new MyLine(a1, a2);
            MyLine l23 = new MyLine(a2, a3);
            MyLine l34 = new MyLine(a3, a4);
            MyLine l41 = new MyLine(a4, a1);

            MyLine l35 = new MyLine(a3, a5);
            MyLine l56 = new MyLine(a5, a6);
            MyLine l64 = new MyLine(a6, a4);

            MyLine l27 = new MyLine(a2, a7);
            MyLine l78 = new MyLine(a7, a8);
            MyLine l83 = new MyLine(a8, a3);

            MyLine l89 = new MyLine(a8, a9);
            MyLine l95 = new MyLine(a9, a5);

            modelLines.Add(l41);
            modelLines.Add(l34);
            modelLines.Add(l23);
            modelLines.Add(l12);
            modelLines.Add(l35);
            modelLines.Add(l56);
            modelLines.Add(l64);
            modelLines.Add(l27);
            modelLines.Add(l78);
            modelLines.Add(l83);
            modelLines.Add(l89);
            modelLines.Add(l95);


            MyRoom first = new MyRoom("FirstRoom", "1", RoomType.Kitchen);
            MyRoom second = new MyRoom("SecondRoom", "2", RoomType.LivingRoom);
            MyRoom third = new MyRoom("ThirdRoom", "3", RoomType.CorridorRoom);
            MyRoom fourth = new MyRoom("FourthRoom", "4", RoomType.RestRoom);

            foreach (MyLine modelLine in new List<MyLine>() { l12, l23, l34, l41 }) {
                modelLine.relatedRooms.Add(first);
            }

            foreach (MyLine modelLine in new List<MyLine>() { l35, l56, l64, l34 }) {
                modelLine.relatedRooms.Add(second);
            }
            foreach (MyLine modelLine in new List<MyLine>() { l23, l27, l78, l83 }) {
                modelLine.relatedRooms.Add(third);
            }
            foreach (MyLine modelLine in new List<MyLine>() { l83, l89, l95, l35 }) {
                modelLine.relatedRooms.Add(fourth);
            }


            CalculateAllRooms();
            Logger.WriteLog("InitSimpleModel() finished");
        }
        public void InitSkewedModel() {
            loadedModelType = ModelType.Skewed;
            modelLines = new ObservableCollection<MyLine>();
            modelRooms = new ObservableCollection<MyRoom>();
            MyPoint a1 = new MyPoint(0, 0);
            MyPoint a2 = new MyPoint(300, 0);
            MyPoint a3 = new MyPoint(300, 300);
            MyPoint a4 = new MyPoint(0, 300);

            MyPoint a5 = new MyPoint(300, 400);
            MyPoint a6 = new MyPoint(0, 400);

            MyPoint a7 = new MyPoint(400, 0);
            MyPoint a8 = new MyPoint(400, 300);

            MyPoint a9 = new MyPoint(400, 400);

            MyLine l12 = new MyLine(a1, a2);
            MyLine l23 = new MyLine(a2, a3);
            MyLine l34 = new MyLine(a3, a4);
            MyLine l41 = new MyLine(a4, a1);

            MyLine l35 = new MyLine(a3, a5);
            MyLine l56 = new MyLine(a5, a6);
            MyLine l64 = new MyLine(a6, a4);

            MyLine l27 = new MyLine(a2, a7);
            MyLine l78 = new MyLine(a7, a8);
            MyLine l83 = new MyLine(a8, a3);

            MyLine l89 = new MyLine(a8, a9);
            MyLine l95 = new MyLine(a9, a5);

            modelLines.Add(l41);
            modelLines.Add(l34);
            modelLines.Add(l23);
            modelLines.Add(l12);
            modelLines.Add(l35);
            modelLines.Add(l56);
            modelLines.Add(l64);
            modelLines.Add(l27);
            modelLines.Add(l78);
            modelLines.Add(l83);
            modelLines.Add(l89);
            modelLines.Add(l95);


            MyRoom first = new MyRoom("FirstRoom", "1", RoomType.Kitchen);
            MyRoom second = new MyRoom("SecondRoom", "2", RoomType.LivingRoom);
            MyRoom third = new MyRoom("ThirdRoom", "3", RoomType.BedRoom);
            MyRoom fourth = new MyRoom("FourthRoom", "4", RoomType.RestRoom);

            foreach (MyLine modelLine in new List<MyLine>() { l12, l23, l34, l41 }) {
                modelLine.relatedRooms.Add(first);
            }

            foreach (MyLine modelLine in new List<MyLine>() { l35, l56, l64, l34 }) {
                modelLine.relatedRooms.Add(second);
            }
            foreach (MyLine modelLine in new List<MyLine>() { l23, l27, l78, l83 }) {
                modelLine.relatedRooms.Add(third);
            }
            foreach (MyLine modelLine in new List<MyLine>() { l83, l89, l95, l35 }) {
                modelLine.relatedRooms.Add(fourth);
            }


            CalculateAllRooms();
            Logger.WriteLog("InitSkewedModel() finished");
        }
        public void InitAdvancedModel() {
            loadedModelType = ModelType.Advanced;
            modelLines = new ObservableCollection<MyLine>();
            modelRooms = new ObservableCollection<MyRoom>();
            MyPoint a1 = new MyPoint(000, 000);
            MyPoint a2 = new MyPoint(200, 000);
            MyPoint a3 = new MyPoint(200, 200);
            MyPoint a4 = new MyPoint(000, 200);

            MyPoint a5 = new MyPoint(200, 400);
            MyPoint a6 = new MyPoint(000, 400);

            MyPoint a7 = new MyPoint(400, 000);
            MyPoint a8 = new MyPoint(400, 200);

            MyPoint a9 = new MyPoint(400, 400);

            MyLine l12 = new MyLine(a1, a2);
            MyLine l23 = new MyLine(a2, a3);
            MyLine l34 = new MyLine(a3, a4);
            MyLine l41 = new MyLine(a4, a1);

            MyLine l35 = new MyLine(a3, a5);
            MyLine l56 = new MyLine(a5, a6);
            MyLine l64 = new MyLine(a6, a4);

            MyLine l27 = new MyLine(a2, a7);
            MyLine l78 = new MyLine(a7, a8);
            MyLine l83 = new MyLine(a8, a3);

            MyLine l89 = new MyLine(a8, a9);
            MyLine l95 = new MyLine(a9, a5);

            modelLines.Add(l41);
            modelLines.Add(l34);
            modelLines.Add(l23);
            modelLines.Add(l12);
            modelLines.Add(l35);
            modelLines.Add(l56);
            modelLines.Add(l64);
            modelLines.Add(l27);
            modelLines.Add(l78);
            modelLines.Add(l83);
            modelLines.Add(l89);
            modelLines.Add(l95);


            MyRoom first = new MyRoom("FirstRoom", "1", RoomType.CorridorRoom);
            MyRoom second = new MyRoom("SecondRoom", "2", RoomType.BedRoom);
            MyRoom third = new MyRoom("ThirdRoom", "3", RoomType.CorridorRoom);
            MyRoom fourth = new MyRoom("FourthRoom", "4", RoomType.RestRoom);

            foreach (MyLine modelLine in new List<MyLine>() { l12, l23, l34, l41 }) {
                modelLine.relatedRooms.Add(first);
            }

            foreach (MyLine modelLine in new List<MyLine>() { l35, l56, l64, l34 }) {
                modelLine.relatedRooms.Add(second);
            }
            foreach (MyLine modelLine in new List<MyLine>() { l23, l27, l78, l83 }) {
                modelLine.relatedRooms.Add(third);
            }
            foreach (MyLine modelLine in new List<MyLine>() { l83, l89, l95, l35 }) {
                modelLine.relatedRooms.Add(fourth);
            }


            MyPoint _a1 = new MyPoint(-000, -000);
            MyPoint _a2 = new MyPoint(-300, -000);
            MyPoint _a3 = new MyPoint(-300, -300);
            MyPoint _a4 = new MyPoint(-000, -300);
            MyPoint _a5 = new MyPoint(-300, -400);
            MyPoint _a6 = new MyPoint(-000, -400);
            MyPoint _a7 = new MyPoint(-400, -000);
            MyPoint _a8 = new MyPoint(-400, -300);
            MyPoint _a9 = new MyPoint(-400, -400);

            MyLine _l12 = new MyLine(a1, _a2); //i modofied it from _a1
            MyLine _l23 = new MyLine(_a2, _a3);
            MyLine _l34 = new MyLine(_a3, _a4);
            MyLine _l41 = new MyLine(_a4, a1); //i modofied it from _a1
            MyLine _l35 = new MyLine(_a3, _a5);
            MyLine _l56 = new MyLine(_a5, _a6);
            MyLine _l64 = new MyLine(_a6, _a4);
            MyLine _l27 = new MyLine(_a2, _a7);
            MyLine _l78 = new MyLine(_a7, _a8);
            MyLine _l83 = new MyLine(_a8, _a3);
            MyLine _l89 = new MyLine(_a8, _a9);
            MyLine _l95 = new MyLine(_a9, _a5);

            modelLines.Add(_l41);
            modelLines.Add(_l34);
            modelLines.Add(_l23);
            modelLines.Add(_l12);
            modelLines.Add(_l35);
            modelLines.Add(_l56);
            modelLines.Add(_l64);
            modelLines.Add(_l27);
            modelLines.Add(_l78);
            modelLines.Add(_l83);
            modelLines.Add(_l89);
            modelLines.Add(_l95);


            MyRoom _first = new MyRoom("_FirstRoom", "_1", RoomType.Kitchen);
            MyRoom _second = new MyRoom("_SecondRoom", "_2", RoomType.LivingRoom);
            MyRoom _third = new MyRoom("_ThirdRoom", "_3", RoomType.BedRoom);
            MyRoom _fourth = new MyRoom("_FourthRoom", "_4", RoomType.RestRoom);

            foreach (MyLine modelLine in new List<MyLine>() { _l12, _l23, _l34, _l41 }) {
                modelLine.relatedRooms.Add(_first);
            }

            foreach (MyLine modelLine in new List<MyLine>() { _l35, _l56, _l64, _l34 }) {
                modelLine.relatedRooms.Add(_second);
            }
            foreach (MyLine modelLine in new List<MyLine>() { _l23, _l27, _l78, _l83 }) {
                modelLine.relatedRooms.Add(_third);
            }
            foreach (MyLine modelLine in new List<MyLine>() { _l83, _l89, _l95, _l35 }) {
                modelLine.relatedRooms.Add(_fourth);
            }

            //modelRooms = new ObservableCollection<MyRoom>(new List<MyRoom>(){first, second, third, fourth, _first, _second, _third, _fourth});
            CalculateAllRooms();
            //RepairDuplicatePointsAndLines();
            Logger.WriteLog("Advanced model initialized");
        }
        public void InitModelWithGivenRooms() {
            loadedModelType = ModelType.Specified;
            modelLines = new ObservableCollection<MyLine>();
            modelRooms = new ObservableCollection<MyRoom>();
            List<Tuple<FactoryRoomType, int>> initList = new List<Tuple<FactoryRoomType, int>>();
            initList.Add(Tuple.Create(FactoryRoomType.DobozosMagasraktar, 1));
            initList.Add(Tuple.Create(FactoryRoomType.Trafo, 2));
            initList.Add(Tuple.Create(FactoryRoomType.KapcsoloHelyiseg0_4kv, 2));
            initList.Add(Tuple.Create(FactoryRoomType.KapcsoloHelyiseg20kv, 2));
            initList.Add(Tuple.Create(FactoryRoomType.SprinklerHelyiseg, 1));
            initList.Add(Tuple.Create(FactoryRoomType.GeneratorHelyiseg, 1));
            initList.Add(Tuple.Create(FactoryRoomType.GepeszetiHelyiseg, 1));

            List<MyRoom> rooms = new List<MyRoom>();
            foreach (Tuple<FactoryRoomType, int> tuple in initList) {
                for (int i = 0; i < tuple.Item2; i++) {
                    MyRoom r = new MyRoom(tuple.Item1.roomname + i, i.ToString(), tuple.Item1);
                    rooms.Add(r);
                }
            }

            int x0 = 0;
            int y0 = 0;
            int side_length = 200;
            foreach (MyRoom room in rooms) {
                MyPoint p1 = new MyPoint(x0, y0);
                MyPoint p2 = new MyPoint(x0 + side_length, y0);
                MyPoint p3 = new MyPoint(x0 + side_length, y0 + side_length);
                MyPoint p4 = new MyPoint(x0, y0 + side_length);

                MyLine line1 = new MyLine(p1, p2);
                MyLine line2 = new MyLine(p2, p3);
                MyLine line3 = new MyLine(p3, p4);
                MyLine line4 = new MyLine(p4, p1);
                modelLines.Add(line1);
                modelLines.Add(line2);
                modelLines.Add(line3);
                modelLines.Add(line4);

                foreach (MyLine modelLine in new List<MyLine>() { line1, line2, line3, line4 }) {
                    modelLine.relatedRooms.Add(room);
                }
                modelRooms.Add(room);
                x0 += 200;
            }
            CalculateAllRooms();
            RepairDuplicatePointsAndLines();
            //CleanDuplicatePoints();
            CalculateAllRooms();

        }

        public void InitTestModel()
        {
            loadedModelType = ModelType.Normal;
            modelLines = new ObservableCollection<MyLine>();
            modelRooms = new ObservableCollection<MyRoom>();
            MyPoint a1 = new MyPoint(0, 0);
            MyPoint a2 = new MyPoint(0, 100);
            MyPoint a3 = new MyPoint(0, 200);
            MyPoint a4 = new MyPoint(0, 300);

            MyPoint a5 = new MyPoint(100, 0);
            MyPoint a6 = new MyPoint(100, 100);
            MyPoint a7 = new MyPoint(100, 200);
            MyPoint a8 = new MyPoint(100, 300);

            MyPoint a9  = new MyPoint(200, 0);
            MyPoint a10 = new MyPoint(200, 100);
            MyPoint a11 = new MyPoint(200, 200);
            MyPoint a12 = new MyPoint(200, 300);

            MyPoint a13 = new MyPoint(300, 0);
            MyPoint a14 = new MyPoint(300, 100);
            MyPoint a15 = new MyPoint(300, 200);
            MyPoint a16 = new MyPoint(300, 300);

           

            MyLine l12 = new MyLine(a1, a2);
            MyLine l23 = new MyLine(a2, a3);
            MyLine l34 = new MyLine(a3, a4);

            MyLine l56 = new MyLine(a5, a6);
            MyLine l67 = new MyLine(a6, a7);
            MyLine l78 = new MyLine(a7, a8);

            MyLine l910 = new MyLine(a9, a10);
            MyLine l1011 = new MyLine(a10, a11);
            MyLine l1112 = new MyLine(a11, a12);

            MyLine l1314 = new MyLine(a13, a14);
            MyLine l1415 = new MyLine(a14, a15);
            MyLine l1516 = new MyLine(a15, a16);

            
            MyLine l15 = new MyLine(a1, a5);
            MyLine l59 = new MyLine(a5, a9);
            MyLine l913 = new MyLine(a9, a13);

            MyLine l26 = new MyLine(a2, a6);
            MyLine l610 = new MyLine(a6, a10);
            MyLine l1014 = new MyLine(a10, a14);

            MyLine l37 = new MyLine(a3, a7);
            MyLine l711 = new MyLine(a7, a11);
            MyLine l1115 = new MyLine(a11, a15);

            MyLine l48 = new MyLine(a4, a8);
            MyLine l812 = new MyLine(a8, a12);
            MyLine l1216 = new MyLine(a12, a16);


            modelLines.Add(l12);
            modelLines.Add(l23);
            modelLines.Add(l34);
            modelLines.Add(l56);
            modelLines.Add(l67);
            modelLines.Add(l78);
            modelLines.Add(l910);
            modelLines.Add(l1011);
            modelLines.Add(l1112);
            modelLines.Add(l1314);
            modelLines.Add(l1415);
            modelLines.Add(l1516);


            modelLines.Add(l15);
            modelLines.Add(l59);
            modelLines.Add(l913);
            modelLines.Add(l26);
            modelLines.Add(l610);
            modelLines.Add(l1014);
            modelLines.Add(l37);
            modelLines.Add(l711);
            modelLines.Add(l1115);
            modelLines.Add(l48);
            modelLines.Add(l812);
            modelLines.Add(l1216);

            MyRoom first = new MyRoom("FirstRoom", "1", RoomType.Kitchen);
            MyRoom second = new MyRoom("SecondRoom", "2", RoomType.LivingRoom);
            second.isStartRoom = true;
            MyRoom third = new MyRoom("ThirdRoom", "3", RoomType.CorridorRoom);
            MyRoom fourth = new MyRoom("FourthRoom", "4", RoomType.RestRoom);
            MyRoom fifth = new MyRoom("fifthRoom", "5", RoomType.BedRoom);
            MyRoom sixth = new MyRoom("sixthRoom", "6", RoomType.BedRoom);
            MyRoom seventh = new MyRoom("seventhRoom", "7", RoomType.BedRoom);
            MyRoom eight = new MyRoom("eightRoom", "8", RoomType.BedRoom);
            MyRoom nineth = new MyRoom("ninethRoom", "9", RoomType.BedRoom);

            foreach (MyLine modelLine in new List<MyLine>() { l12, l26, l56, l15 }) {
                modelLine.relatedRooms.Add(first);
            }
            foreach (MyLine modelLine in new List<MyLine>() { l23, l37, l67, l26 }) {
                modelLine.relatedRooms.Add(second);
            }
            foreach (MyLine modelLine in new List<MyLine>() { l34, l48, l78, l37 }) {
                modelLine.relatedRooms.Add(third);
            }
            foreach (MyLine modelLine in new List<MyLine>() { l56, l610, l910, l59 }) {
                modelLine.relatedRooms.Add(fourth);
            }
            foreach (MyLine modelLine in new List<MyLine>() { l67, l711, l1011, l610 }) {
                modelLine.relatedRooms.Add(fifth);
            }
            foreach (MyLine modelLine in new List<MyLine>() { l78, l812, l1112, l711 }) {
                modelLine.relatedRooms.Add(sixth);
            }

            foreach (MyLine modelLine in new List<MyLine>() { l910, l1014, l1314, l913 }) {
                modelLine.relatedRooms.Add(seventh);
            }
            foreach (MyLine modelLine in new List<MyLine>() { l1011, l1115, l1415, l1014 }) {
                modelLine.relatedRooms.Add(eight);
            }
            foreach (MyLine modelLine in new List<MyLine>() { l1112, l1216, l1115, l1516 }) {
                modelLine.relatedRooms.Add(nineth);
            }


            CalculateAllRooms();
            Logger.WriteLog("InitSimpleModel() finished");
        }

        [JsonIgnore]
        [IgnoreDataMember]
        private Dictionary<MyRoom, MyRoom> oldNewRooms = new Dictionary<MyRoom, MyRoom>();
        [JsonIgnore]
        [IgnoreDataMember]
        private Dictionary<MyPoint, MyPoint> oldNewPoints = new Dictionary<MyPoint, MyPoint>();
        [JsonIgnore]
        [IgnoreDataMember]
        private Dictionary<MyLine, MyLine> oldNewLines = new Dictionary<MyLine, MyLine>();
        /// <summary>
        /// Base deepcopy with no other returning parameters
        /// </summary>
        /// <returns></returns>
        public void LoadData() {
            oldNewRooms = new Dictionary<MyRoom, MyRoom>();
            oldNewPoints = new Dictionary<MyPoint, MyPoint>();
            oldNewLines = new Dictionary<MyLine, MyLine>();
            foreach (MyLine line in modelLines) {
                MyPoint p1 = null;
                MyPoint p2 = null;
                //this block is responsible to copy the point if it does not already exist - it also handles duplicates, maybe shouldnt?
                if (!oldNewPoints.TryGetValue(line.StartMyPoint, out p1)) {
                    p1 = line.StartMyPoint.GetCopy();
                    oldNewPoints.Add(line.StartMyPoint, p1);
                }
                if (!oldNewPoints.TryGetValue(line.EndMyPoint, out p2)) {
                    p2 = line.EndMyPoint.GetCopy();
                    oldNewPoints.Add(line.EndMyPoint, p2);
                }
                //now the points must exist, so here is a nullcheck 
                if (p1 == null || p2 == null) {
                    throw new Exception("Points must exist at this point");
                }
                MyLine newline = new MyLine(p1, p2);
                oldNewLines.Add(line, newline);

                foreach (MyRoom room in line.relatedRooms) {
                    MyRoom r = null;
                    if (!oldNewRooms.TryGetValue(room, out r)) {
                        //TODO: this getcopy method might not return all good. check it.
                        r = room.GetCopy();
                        oldNewRooms.Add(room, r);
                    }
                    newline.relatedRooms.Add(r);
                }
            }

            foreach (KeyValuePair<MyRoom, MyRoom> oldNew in oldNewRooms) {
                MyRoom oldMyRoom = oldNew.Key;
                MyRoom newMyRoom = oldNew.Value;
                newMyRoom.BoundaryLines.Clear();
                foreach (MyLine oldLine in oldMyRoom.BoundaryLines) {
                    //TODO: some line is already registered to the room
                    //the issue is line cancellation, when a line is gone, even in a simple move situation
                    try {
                        newMyRoom.BoundaryLines.Add(oldNewLines[oldLine]);
                    }
                    catch (Exception) {

                    }
                }
            }
        }
        public Model DeepCopy() {
            LoadData();
            return new Model(oldNewLines.Values.ToList(), oldNewRooms.Values.ToList());
        }
        public Model DeepCopy(MyLine oldMyLine, out MyLine newMyLine) {
            //Dictionary<MyRoom, MyRoom> oldNewRooms = new Dictionary<MyRoom, MyRoom>();
            //Dictionary<MyPoint, MyPoint> oldNewPoints = new Dictionary<MyPoint, MyPoint>();
            //Dictionary<MyLine, MyLine> oldNewLines = new Dictionary<MyLine, MyLine>();
            LoadData();

            //foreach (MyLine line in modelLines) {
            //    MyPoint p1 = null;
            //    MyPoint p2 = null;
            //    if (!oldNewPoints.TryGetValue(line.StartMyPoint, out p1)) {
            //        p1 = line.StartMyPoint.GetCopy();
            //        oldNewPoints.Add(line.StartMyPoint, p1);
            //    }
            //    if (!oldNewPoints.TryGetValue(line.EndMyPoint, out p2)) {
            //        p2 = line.EndMyPoint.GetCopy();
            //        oldNewPoints.Add(line.EndMyPoint, p2);
            //    }
            //    MyLine l = new MyLine(p1, p2);
            //    oldNewLines.Add(line, l);
            //    foreach (MyRoom room in line.relatedRooms) {
            //        MyRoom r = null;
            //        if (!oldNewRooms.TryGetValue(room, out r)) {
            //            r = room.GetCopy();
            //            oldNewRooms.Add(room, r);
            //        }
            //        l.relatedRooms.Add(r);
            //    }
            //}
            //foreach (KeyValuePair<MyRoom, MyRoom> oldNew in oldNewRooms) {
            //    MyRoom oldRoom = oldNew.Key;
            //    MyRoom newRoom = oldNew.Value;
            //    foreach (MyLine oldLine in oldRoom.BoundaryLines) {
            //        //TODO: some line is already registered to the room
            //        newRoom.BoundaryLines.Add(oldNewLines[oldLine]);
            //    }
            //}

            newMyLine = oldNewLines[oldMyLine];

            return new Model(oldNewLines.Values.ToList(), oldNewRooms.Values.ToList());
        }
        public Model DeepCopy(MyRoom oldMyRoom1, MyRoom oldMyRoom2, out MyRoom newMyRoom1, out MyRoom newMyRoom2) {
            //Dictionary<MyRoom, MyRoom> oldNewRooms = new Dictionary<MyRoom, MyRoom>();
            //Dictionary<MyPoint, MyPoint> oldNewPoints = new Dictionary<MyPoint, MyPoint>();
            //Dictionary<MyLine, MyLine> oldNewLines = new Dictionary<MyLine, MyLine>();
            LoadData();
            //foreach (MyLine line in modelLines) {
            //    MyPoint p1 = null;
            //    MyPoint p2 = null;

            //    if (!oldNewPoints.TryGetValue(line.StartMyPoint, out p1)) {
            //        p1 = line.StartMyPoint.GetCopy();
            //        oldNewPoints.Add(line.StartMyPoint, p1);
            //    }

            //    if (!oldNewPoints.TryGetValue(line.EndMyPoint, out p2)) {
            //        p2 = line.EndMyPoint.GetCopy();
            //        oldNewPoints.Add(line.EndMyPoint, p2);
            //    }
            //    MyLine l = new MyLine(p1, p2);
            //    oldNewLines.Add(line, l);

            //    foreach (MyRoom room in line.relatedRooms) {
            //        MyRoom r = null;
            //        if (!oldNewRooms.TryGetValue(room, out r)) {
            //            r = room.GetCopy();
            //            oldNewRooms.Add(room, r);
            //        }
            //        l.relatedRooms.Add(r);
            //    }
            //}

            //foreach (KeyValuePair<MyRoom, MyRoom> oldNew in oldNewRooms) {
            //    MyRoom oldRoom = oldNew.Key;
            //    MyRoom newRoom = oldNew.Value;

            //    foreach (MyLine oldLine in oldRoom.BoundaryLines) {
            //        //TODO: some line is already registered to the room
            //        //try {

            //        newRoom.BoundaryLines.Add(oldNewLines[oldLine]);
            //        //}
            //        //catch (Exception ex) {
            //        //    Logger.WriteLog(ex);
            //        //}

            //    }
            //}

            newMyRoom1 = oldNewRooms[oldMyRoom1];
            newMyRoom2 = oldNewRooms[oldMyRoom2];

            return new Model(oldNewLines.Values.ToList(), oldNewRooms.Values.ToList());
        }

        public Model DeepCopy(MyRoom oldMyRoom1, out MyRoom newMyRoom1) {
           
            LoadData();
            
            newMyRoom1 = oldNewRooms[oldMyRoom1];

            return new Model(oldNewLines.Values.ToList(), oldNewRooms.Values.ToList());
        }

        //the cause of all prolems is in this function.
        //but it is not the function that is wrong, it makes the model in an invalid state, but that can happen on other functions aswell.
        public void RepairDuplicatePointsAndLines() {
            CleanDuplicatePoints();
            CleanDuplicateLines();
            CleanOverlappingLines();
        }

        private void CleanDuplicateLines() {
            //ha teljes vonalat ki tudok cserélni, akkor cseréljem is ki.
            List<List<MyLine>> linestoreplace = new List<List<MyLine>>();

            Dictionary<MyLine, List<MyLine>> linesToRemoveDict = new Dictionary<MyLine, List<MyLine>>();
            List<MyLine> linesToRemove = new List<MyLine>();
            foreach (MyLine modelLine in modelLines) {
                if (linesToRemove.Contains(modelLine)) continue;

                List<MyLine> mach = modelLines.Where(i => (modelLine.StartMyPoint.Equals(i.StartMyPoint) &&
                                                           modelLine.EndMyPoint.Equals(i.EndMyPoint)) ||
                                                          (modelLine.StartMyPoint.Equals(i.EndMyPoint) &&
                                                           modelLine.EndMyPoint.Equals(i.StartMyPoint))
                ).ToList();

                if (mach.Contains(modelLine)) {
                    mach.Remove(modelLine);
                }

                if (mach.Count == 0) continue;
                linesToRemove.AddRange(mach);
                linesToRemoveDict.Add(modelLine, mach);
            }

            foreach (var myLine in linesToRemoveDict) {
                foreach (MyLine line in myLine.Value) {
                    foreach (MyRoom lineRelatedRoom in line.relatedRooms) {
                        lineRelatedRoom.BoundaryLines.Remove(line);
                        lineRelatedRoom.BoundaryLines.Add(myLine.Key);
                        myLine.Key.relatedRooms.Add(lineRelatedRoom);
                    }

                    modelLines.Remove(line);
                }
            }
        }

        private void CleanOverlappingLines() {
            List<MyLine> linesToRemove = new List<MyLine>();
            List<MyLine> linesModofied = new List<MyLine>();
            foreach (MyLine testLineB in modelLines) {
                foreach (MyLine lineA in modelLines) {
                    if (testLineB == lineA) continue;
                    if (linesToRemove.Contains(lineA)) continue;

                    bool isStartPointOnMiddleLine = IsOnLine(testLineB.StartMyPoint, lineA);
                    bool isStartPointAtEndLine = testLineB.StartMyPoint.Equals(lineA.StartMyPoint) ||
                                                 testLineB.StartMyPoint.Equals(lineA.EndMyPoint);
                    bool isEndPointOnMiddleLine = IsOnLine(testLineB.EndMyPoint, lineA);
                    bool isEndPointAtEndLine = testLineB.EndMyPoint.Equals(lineA.StartMyPoint) ||
                                               testLineB.EndMyPoint.Equals(lineA.EndMyPoint);


                    if (isStartPointAtEndLine && isEndPointAtEndLine) {
                        //then we pass, this is already handled (and should be moved here later)
                    }
                    //CASE 1 complete containing, one point match
                    if (isStartPointAtEndLine && isStartPointOnMiddleLine && isEndPointOnMiddleLine && !isEndPointAtEndLine) {
                        linesToRemove.Add(lineA);
                        if (testLineB.StartMyPoint.Equals(lineA.StartMyPoint)) {
                            testLineB.StartMyPoint.RelatedLines.AddRange(lineA.StartMyPoint.RelatedLines); //give up all the edges
                            lineA.StartMyPoint = testLineB.EndMyPoint;
                            foreach (MyRoom room in testLineB.relatedRooms) {
                                lineA.relatedRooms.Remove(room); //give up all the rooms
                            }
                        }
                        if (testLineB.StartMyPoint.Equals(lineA.EndMyPoint)) {
                            testLineB.StartMyPoint.RelatedLines.AddRange(lineA.EndMyPoint.RelatedLines); //give up all the edges
                            lineA.EndMyPoint = testLineB.EndMyPoint;
                            foreach (MyRoom room in testLineB.relatedRooms) {
                                lineA.relatedRooms.Remove(room); //give up all the rooms
                            }
                        }
                    }
                    //CASE 1 complete containing, one point match - REVERSE
                    if (!isStartPointAtEndLine && isStartPointOnMiddleLine && isEndPointOnMiddleLine && isEndPointAtEndLine) {
                        linesToRemove.Add(lineA);
                        if (testLineB.EndMyPoint.Equals(lineA.StartMyPoint)) {
                            testLineB.StartMyPoint.RelatedLines.AddRange(lineA.StartMyPoint.RelatedLines); //give up all the edges
                            lineA.StartMyPoint = testLineB.EndMyPoint;
                            foreach (MyRoom room in testLineB.relatedRooms) {
                                lineA.relatedRooms.Remove(room); //give up all the rooms
                            }
                        }
                        if (testLineB.EndMyPoint.Equals(lineA.EndMyPoint)) {
                            testLineB.StartMyPoint.RelatedLines.AddRange(lineA.EndMyPoint.RelatedLines); //give up all the edges
                            lineA.EndMyPoint = testLineB.EndMyPoint;
                            foreach (MyRoom room in testLineB.relatedRooms) {
                                lineA.relatedRooms.Remove(room); //give up all the rooms
                            }
                        }
                    }

                    //CASE 2 partial containg, no matching point
                    if (!isStartPointAtEndLine && isStartPointOnMiddleLine &&!isEndPointOnMiddleLine && !isEndPointAtEndLine)
                    {
                        if (IsOnLine(lineA.StartMyPoint, testLineB)) //then we know it is really case 2
                        {
                            //throw new Exception("lines overstepped, this should not happen");
                        }

                        if (IsOnLine(lineA.EndMyPoint, testLineB))
                        {
                            //throw new Exception("lines overstepped, this should not happen");
                            
                        }
                    }
                    //CASE 2 partial containg, no matching point - REVERSE
                    if (!isStartPointAtEndLine && !isStartPointOnMiddleLine && isEndPointOnMiddleLine && !isEndPointAtEndLine) {
                        if (IsOnLine(lineA.StartMyPoint, testLineB)) //then we know it is really case 2
                        {
                            //throw new Exception("lines overstepped, this should not happen");
                        }

                        if (IsOnLine(lineA.EndMyPoint, testLineB)) {
                            //throw new Exception("lines overstepped, this should not happen");

                        }
                    }
                    

                    //CASE 3 touching, this should be handled already.


                }

            }
        }


        private void CleanDuplicatePoints() {
            foreach (MyLine modelLine in modelLines) {
                List<MyPoint> asd = ModelPoints.Where(i => i.Equals(modelLine.StartMyPoint)).OrderBy(i => i.Guid).ToList();
                List<MyPoint> asd2 = ModelPoints.Where(i => i.Equals(modelLine.EndMyPoint)).OrderBy(i => i.Guid).ToList();
                if (asd.Count() > 1) {
                    MyPoint replaceTo = asd.First();
                    replaceTo.RelatedLines.AddRange(modelLine.StartMyPoint.RelatedLines); //connect the lines
                    replaceTo.RelatedLines = replaceTo.RelatedLines.Distinct().ToList();
                    modelLine.StartMyPoint = replaceTo;
                }

                if (asd2.Count() > 1) {
                    MyPoint replaceTo = asd2.First();
                    replaceTo.RelatedLines.AddRange(modelLine.EndMyPoint.RelatedLines); //connect the lines
                    replaceTo.RelatedLines = replaceTo.RelatedLines.Distinct().ToList();
                    modelLine.EndMyPoint = replaceTo;
                }
            }
        }
        public MyLine GetRandomLine() {
            int randint = rand.Next(0, modelLines.Count);
            return modelLines.ElementAt(randint);
        }
        public void SplitEdge(int splitPercentage, MyLine selectedEdge) {
            if (modelLines.Count == 0) return;

            double length = selectedEdge.GetLength();
            if (length < 1) return;

            Logger.WriteLog("Selected: " + selectedEdge + " rooms: " + String.Join(",", selectedEdge.relatedRooms.Select(i => i.Name).ToArray()));
            List<MyRoom> selectedEdgeRelatedRooms = selectedEdge.relatedRooms;
            modelLines.Remove(selectedEdge);
            selectedEdge.StartMyPoint.RelatedLines.Remove(selectedEdge);
            selectedEdge.EndMyPoint.RelatedLines.Remove(selectedEdge);

            MyPoint splitMyPoint = selectedEdge.GetPointAt(splitPercentage);
            MyLine a = new MyLine(selectedEdge.StartMyPoint, splitMyPoint);
            a.relatedRooms.AddRange(selectedEdgeRelatedRooms);
            modelLines.Add(a);
            Logger.WriteLog("Added instead: " + a + " rooms: " + String.Join(",", selectedEdge.relatedRooms.Select(i => i.Name).ToArray()));
            MyLine b = new MyLine(splitMyPoint, selectedEdge.EndMyPoint);
            b.relatedRooms.AddRange(selectedEdgeRelatedRooms);
            modelLines.Add(b);
            Logger.WriteLog("Added instead: " + b + " rooms: " + String.Join(",", selectedEdge.relatedRooms.Select(i => i.Name).ToArray()));

            foreach (MyRoom edgeRelatedRoom in selectedEdgeRelatedRooms) {
                try {
                    edgeRelatedRoom.BoundaryLines.Remove(selectedEdge);
                    edgeRelatedRoom.BoundaryLines.Add(a);
                    edgeRelatedRoom.BoundaryLines.Add(b);
                }
                catch (Exception) {

                    Logger.WriteLog("Error: there is something wrong with the split. Check it out.");
                }
            }

            Logger.WriteLog($"Lines to that room {selectedEdge.relatedRooms.First()}: {String.Join(",", selectedEdge.relatedRooms.First().BoundaryLines.Select(i => i))}");
        }
        public void HandleOpening(MyLine l) {
            if (l.relatedRooms.Count == 1) return; //külső fal

            ReachableRooms();

        }

        private void ReachableRooms() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// calculate degree for all rooms starting from the starter room
        /// </summary>
        /// <returns></returns>
        public void SwitchRooms(ref MyRoom room1, ref MyRoom room2) {
            MyRoom temp1 = room1.GetCopy();
            MyRoom temp2 = room2.GetCopy();
            MyRoom.ChangeAllParams(ref room1, temp2);
            MyRoom.ChangeAllParams(ref room2, temp1);
            //make a constructor by another room, or preferably two rooms, and only copy the info of the second, and kepp the reference
        }
        public void MoveLine(int offsetDistance, MyLine myLineToMove) {
            try {
                MyPoint oldStart = myLineToMove.StartMyPoint;
                MyPoint oldEnd = myLineToMove.EndMyPoint;
                MyPoint lineNormal = myLineToMove.GetNV(true);
                #region MoveOrCopyStartPoint

                MyPoint movedStart = oldStart + lineNormal * offsetDistance;
                MyPoint movedEnd = oldEnd + lineNormal * offsetDistance;

                //this ensures, that if the point already exists in the model, we dont create a new.
                MyPoint StartExisting = CheckIfPointIsAlreadyInModel(movedStart);
                MyPoint EndPointExisting = CheckIfPointIsAlreadyInModel(movedEnd);

                ExceptionChecking(offsetDistance, myLineToMove, oldStart, lineNormal, oldEnd);

                bool isStartPointCopied = false;

                foreach (MyLine relatedLine in oldStart.RelatedLines) {
                    //ha van olyan vonal, ami miatt másolni kell:
                    if (!relatedLine.Equals(myLineToMove) && (relatedLine.GetNV(true).Equals(lineNormal)
                                                              || relatedLine.GetNV(true).Equals(lineNormal * (-1)))) {
                        isStartPointCopied = true;
                        break;
                    }
                }
                if (!isStartPointCopied) //then relocate, this is easy //itt szoba alapesetben nem változik, kivéve ha már létezik a 
                {
                    oldStart.X = movedStart.X;
                    oldStart.Y = movedStart.Y;
                }
                else {
                    MyLine myLineInMoveDirection = null;
                    foreach (MyLine relatedLine in oldStart.RelatedLines) {
                        //ha a mozgatás irányába van vonal
                        bool on = IsOnLine(movedStart, relatedLine);
                        if (on) {
                            myLineInMoveDirection = relatedLine;
                            break;
                        }
                    }
                    if (myLineInMoveDirection != null) {
                        if (myLineInMoveDirection.StartMyPoint.Equals(oldStart)) {
                            myLineInMoveDirection.StartMyPoint = movedStart;

                        }
                        else if (myLineInMoveDirection.EndMyPoint.Equals(oldStart)) {
                            myLineInMoveDirection.EndMyPoint = movedStart;
                        }
                        movedStart.RelatedLines.Add(myLineInMoveDirection);
                        oldStart.RelatedLines.Remove(myLineInMoveDirection);
                    }

                    myLineToMove.StartMyPoint = movedStart;
                    movedStart.RelatedLines.Add(myLineToMove);
                    oldStart.RelatedLines.Remove(myLineToMove);

                    List<MyRoom> p1Rooms = oldStart.RelatedRooms;
                    List<MyRoom> p3Rooms = movedStart.RelatedRooms;

                    //this is not always good
                    List<MyRoom> commonRooms = p1Rooms.Intersect(p3Rooms).ToList();

                    MyLine newConnectionEdge1 = new MyLine(oldStart, movedStart);
                    //TODO: add related modelRooms to this new myLine.
                    newConnectionEdge1.relatedRooms = commonRooms;

                    modelLines.Add(newConnectionEdge1);
                }
                #endregion

                #region MoveOrCopyEndPoint
                bool copyp2 = false;
                MyLine parallelLine2 = null;
                foreach (MyLine relatedLine in oldEnd.RelatedLines) {
                    if (!relatedLine.Equals(myLineToMove) &&
                        (relatedLine.GetNV(true).Equals(lineNormal) || relatedLine.GetNV(true).Equals(lineNormal * (-1)))) {
                        copyp2 = true;
                        parallelLine2 = relatedLine;
                        break;
                    }
                }
                if (!copyp2) {
                    oldEnd.X = movedEnd.X;
                    oldEnd.Y = movedEnd.Y;
                }
                else {
                    MyLine myLineInMoveDirection = null;
                    foreach (MyLine relatedLine in oldEnd.RelatedLines) {
                        bool on = IsOnLine(movedEnd, relatedLine);
                        if (on) {
                            myLineInMoveDirection = relatedLine;
                            break;
                        }
                    }
                    if (myLineInMoveDirection != null) {
                        if (myLineInMoveDirection.StartMyPoint.Equals(oldEnd)) {
                            myLineInMoveDirection.StartMyPoint = movedEnd;
                        }
                        else {
                            myLineInMoveDirection.EndMyPoint = movedEnd;
                        }
                        movedEnd.RelatedLines.Add(myLineInMoveDirection);
                        oldEnd.RelatedLines.Remove(myLineInMoveDirection);
                    }

                    myLineToMove.EndMyPoint = movedEnd;

                    movedEnd.RelatedLines.Add(myLineToMove);
                    oldEnd.RelatedLines.Remove(myLineToMove);

                    List<MyRoom> p2Rooms = oldEnd.RelatedRooms;
                    List<MyRoom> p4Rooms = movedEnd.RelatedRooms;

                    List<MyRoom> commonRooms = p2Rooms.Intersect(p4Rooms).ToList();
                    //the new lines related rooms are always handled
                    MyLine newConnectionEdge2 = new MyLine(oldEnd, movedEnd);
                    newConnectionEdge2.relatedRooms = commonRooms;

                    modelLines.Add(newConnectionEdge2);
                }

                #endregion
            }
            catch (Exception e) {
                Logger.WriteLog("Error : Not legal move " + e.Message);
                Logger.WriteLog(e);
                IsInInvalidState = true;
                //throw new Exception("bad");
                //MessageBox.Show();
            }

            CleanupMove();

            //Rooms not handled, but calculaterooms might solve the issue
            CalculateAllRooms();
            CheckModelLinesVerticality();
            RepairDuplicatePointsAndLines();
        }

        private void CleanupMove() {
            List<MyLine> toremove = new List<MyLine>();
            foreach (MyLine line1 in modelLines) {
                if (line1.StartMyPoint.Equals(line1.EndMyPoint) || Math.Abs(line1.GetLength()) < 0.01) {
                    //ha van olyan vonal, aminek a kezdőpontja az, mint a végpontja, vagy a hossza 0 (ami gyakorlatilag ugyanaz)
                    toremove.Add(line1); //akkor el kell távolítsuk
                    //de ennek az eltávolításnak még végig kell futnia a vonal kapcsolódó pontjain
                    //az eltávolítandó vonal végpontjába futó pontokra megyünk rá most
                    foreach (MyLine endLine in line1.EndMyPoint.RelatedLines) {
                        //értelemszerűen ki kell hagyni azt a vonalat, amit most eltávolítunk
                        if (endLine != line1) {
                            //a vonal kezdőpontjához hozzáadjuk az új vonalakat, amik ugye belemennek
                            line1.StartMyPoint.RelatedLines.Add(endLine);
                            //ezután pedig megpróbáljuk átkötni a vonalakat
                            if (endLine.StartMyPoint == line1.EndMyPoint) {
                                // ez az ág akkor fut le, ha a kezdőpontot kell cserélni
                                endLine.StartMyPoint = line1.StartMyPoint;
                            }
                            else if (endLine.EndMyPoint == line1.EndMyPoint) {
                                // ez az ág akkor fut le, ha a végpontot kell cserélni
                                endLine.EndMyPoint = line1.StartMyPoint;
                            }
                        }
                    }

                    //ez nem kezeli egyelőre, hogy a vonal egy szoba tagja volt

                    //a kezdőpontból eltávolítjuk a linet (és nem kéne hozzáadni az összes új vonalat?)
                    line1.StartMyPoint.RelatedLines.AddRange(line1.EndMyPoint.RelatedLines);
                    line1.StartMyPoint.RelatedLines.Remove(line1); //
                    line1.StartMyPoint.RelatedLines.Remove(line1); //
                    line1.EndMyPoint.RelatedLines.Clear(); //ez meg csak biztonság
                }
            }

            foreach (var line2 in toremove) {
                modelLines.Remove(line2);
                //this should be removed from all rooms which contain this line
            }

            foreach (var modelRoom in modelRooms) {
                foreach (MyLine line in toremove) {
                    if (modelRoom.BoundaryLines.Contains(line)) {
                        modelRoom.BoundaryLines.Remove(line);
                    }
                }
            }
        }

        private void CheckModelLinesVerticality() {
            foreach (MyLine line in modelLines) {
                if (!IsVerticalOrHorizontal(line)) {
                    IsInInvalidState = true;
                    break;
                }
            }
        }

        private bool IsVerticalOrHorizontal(MyLine line) {
            bool isVertical = (line.StartMyPoint.Y - line.EndMyPoint.Y) < 0.001;
            bool isHorizontal = (line.StartMyPoint.X - line.EndMyPoint.X) < 0.001;

            return isVertical || isHorizontal;
        }
        private MyPoint CheckIfPointIsAlreadyInModel(MyPoint myPoint) {
            MyPoint local = myPoint.GetCopy();
            bool contains = GetUniqueModelPoints().Contains(myPoint);
            List<MyPoint> tempPoints = GetUniqueModelPoints().Where(i => i.Equals(local)).ToList();
            bool isSame = tempPoints.Any();

            if (tempPoints.Count > 1) {
                throw new Exception("The model state was not valid before check: two points at same place");
            }

            if (isSame) {
                local = tempPoints.FirstOrDefault();
            }

            return local;
        }
        private static void ExceptionChecking(int offsetDistance, MyLine myLineToMove, MyPoint oldStartPoint,
            MyPoint lineToMoveNormal, MyPoint oldEndPoint) {
            foreach (MyLine relatedLine in oldStartPoint.RelatedLines) {
                if (!relatedLine.Equals(myLineToMove) && (!relatedLine.GetNV(true).Equals(lineToMoveNormal)
                                                          || !relatedLine.GetNV(true).Equals(lineToMoveNormal * (-1)))) {
                    if (relatedLine.GetLength() < offsetDistance) {
                        //throw new Exception("Vonalhossz hiba: " + relatedLine.GetLength());
                    }
                }
            }

            foreach (MyLine relatedLine in oldEndPoint.RelatedLines) {
                if (!relatedLine.Equals(myLineToMove) && (!relatedLine.GetNV(true).Equals(lineToMoveNormal)
                                                          || !relatedLine.GetNV(true).Equals(lineToMoveNormal * (-1)))) {
                    if (relatedLine.GetLength() < offsetDistance) {
                        //throw new Exception("Hiba");
                    }
                }
            }
        }


        //TODO: this could be calculated for only the lines, that actually changed, this is huge resource waste
        public void CalculateAllRooms() {
            //itt kezelni lehet majd azt a kérdést, hogy van a line-hoz hozzárendelt olyan szoba is, ami nem létezik már
            modelRooms.Clear();
            List<MyRoom> allUniqueRooms = new List<MyRoom>();
            foreach (MyLine line in modelLines) {
                //minden modellinera megnézzük, hogy
                //annak a hozzá tartozó relatedroomjaiban a room boundarylinejai kozott szerepel-e a myLine
                foreach (MyRoom room in line.relatedRooms.ToList()) {
                    if (!allUniqueRooms.Select(i=>i.Name).Contains(room.Name)) {
                        allUniqueRooms.Add(room);
                        if (!room.BoundaryLines.Contains(line)) {
                            room.BoundaryLines.Add(line); //Logger.WriteLog($"CalculateAllRooms for myLine {myLine} {room.Name} ");
                        }
                    }
                    else
                    {
                        line.relatedRooms.Remove(room);
                        line.relatedRooms.Add(allUniqueRooms.FirstOrDefault(i=>i.Name.Equals(room.Name)));
                        allUniqueRooms.FirstOrDefault(i => i.Name.Equals(room.Name))?.BoundaryLines.Add(line);
                    }
                }
            }
            modelRooms = new ObservableCollection<MyRoom>(allUniqueRooms);//modelRooms.Distinct().ToList();
        }
        private List<List<MyLine>> CalculateContaining(MyLine line1, MyLine line2) {

            List<List<MyLine>> results = new List<List<MyLine>>();
            List<MyLine> addLines = new List<MyLine>();
            List<MyLine> remLines = new List<MyLine>();
            bool isSameDir = line1.GetNV(true).Equals(line2.GetNV(true));
            bool isOppDir = (line1.GetNV(true) * (-1)).Equals(line2.GetNV(true));
            if (isOppDir || isSameDir) {
                bool line1IncludesLine2 = LineIncludes(line1, line2);
                if (line1IncludesLine2) {
                    remLines.Add(line2);
                }

                bool line2IncludesLine1 = LineIncludes(line2, line1);
                if (line2IncludesLine1) {
                    remLines.Add(line1);
                }

                if (!line2IncludesLine1 && !line1IncludesLine2) {
                    MyLine newMyLine = LineHaveCommonPoint(line1, line2);

                    if (newMyLine != null) {
                        remLines.Add(line1);
                        remLines.Add(line2);
                        addLines.Add(newMyLine);
                    }
                }
            }
            results.Add(addLines);
            results.Add(remLines);

            //todo: include partial overlapping
            return results;
        }
        private bool LineIncludes(MyLine line1, MyLine line2) {
            if (IsOnLine(line2.StartMyPoint, line1) && IsOnLine(line2.EndMyPoint, line1)) {
                return true;
            }
            return false;
        }
        private bool IsOnLine(MyPoint myPoint, MyLine myLine) {
            return PointOnLine2D(myPoint, myLine.StartMyPoint, myLine.EndMyPoint);
        }
        public static bool PointOnLine2D(MyPoint p, MyPoint a, MyPoint b, float t = 1E-03f) {
            // ensure points are collinear
            var zero = (b.X - a.X) * (p.Y - a.Y) - (p.X - a.X) * (b.Y - a.Y);
            if (zero > t || zero < -t) return false;

            // check if X-coordinates are not equal
            if (a.X - b.X > t || b.X - a.X > t)
                // ensure X is between a.X & b.X (use tolerance)
                return a.X > b.X
                    ? p.X + t > b.X && p.X - t < a.X
                    : p.X + t > a.X && p.X - t < b.X;

            // ensure Y is between a.Y & b.Y (use tolerance)
            return a.Y > b.Y
                ? p.Y + t > b.Y && p.Y - t < a.Y
                : p.Y + t > a.Y && p.Y - t < b.Y;
        }
        private MyLine LineHaveCommonPoint(MyLine line1, MyLine line2) {
            MyPoint s1 = line1.StartMyPoint;
            MyPoint s2 = line2.StartMyPoint;
            MyPoint e1 = line1.EndMyPoint;
            MyPoint e2 = line2.EndMyPoint;
            MyLine newMyLine = null;
            if (s1.Equals(s2)) {
                newMyLine = new MyLine(e1, e2);
            }
            else if (e1.Equals(e2)) {
                newMyLine = new MyLine(s1, s2);
            }
            else if (e1.Equals(s2)) {
                newMyLine = new MyLine(e2, s1);
            }
            else if (s1.Equals(e2)) {
                newMyLine = new MyLine(e1, s2);
            }

            return newMyLine;
        }



    }
}