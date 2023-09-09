using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using foundry;
using YAXLib;
using YAXLib.Attributes;
using YAXLib.Enums;
using WeifenLuo.WinFormsUI.Docking;
using static foundry.unit.UnitModule;

namespace foundry.unit
{
    public class UnitModule : BaseModule 
    {
        public override Type PageType { get { return typeof(BaseEditorPage); } }
        protected override void OnInit()
        {
            Operators_UnitRightClicked = new OperatorRegistrantToolstrip();
        }
        protected override void OnWorkspaceOpened()
        {
            LoadUnits();
        }
        protected override void OnWorkspaceClosed()
        {
            Units.Clear();
            Squads.Clear();
        }


        public List<UnitPickerPage> UnitPickers { get; } = new List<UnitPickerPage>();
        public void UpdateUnitPickers()
        {
            UnitPickersUpdatedArgs args = new UnitPickersUpdatedArgs()
            {

            };
            UnitPickersUpdated(this, args);
        }
        public class UnitPickersUpdatedArgs
        {

        }
        public event EventHandler<UnitPickersUpdatedArgs> UnitPickersUpdated;


        #region object/unit
        [YAXSerializeAs("Object")]
        public class Unit
        {
            public class EditorDataClass
            {
                [YAXCollection(YAXCollectionSerializationTypes.Serially,
                   SeparateBy = "/")]
                [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
                public List<string> Group { get; set; }
            }
            [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
            public EditorDataClass EditorData { get; set; }


            [YAXAttributeForClass]
            [YAXSerializeAs("name")]
            [YAXErrorIfMissed(YAXExceptionTypes.Error)]
            public string Name { get; set; }

            public enum ObjectClassEnum
            {
                Object,
                Squad,
                Unit,
                Building,
                Projectile,
            }
            [YAXErrorIfMissed(YAXExceptionTypes.Ignore, DefaultValue = ObjectClassEnum.Object)]
            public ObjectClassEnum ObjectClass { get; set; }


            [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
            public string Visual { get; set; }
        }
        public Dictionary<string, Unit> Units { get; set; } = new Dictionary<string, Unit>();
        public Unit SelectedUnit { get; private set; }
        private void LoadUnits()
        {
            string[] files = { "data/objects.xml", "data/objects_update.xml" };

            foreach (string file in files)
            {
                string fullfile = Instance.OpenedWorkspaceDir + file;
                string xml = File.ReadAllText(fullfile);

                YAXSerializer ser = new YAXSerializer(typeof(List<Unit>), new YAXLib.Options.SerializerOptions() 
                {
                    SerializationOptions = YAXSerializationOptions.DontSerializeNullObjects,
                    ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.ThrowWarningsAndErrors
                });
                List<Unit> units = (List<Unit>)ser.Deserialize(xml);

                foreach (Unit unit in units)
                {
                    if (unit.EditorData == null) unit.EditorData = new Unit.EditorDataClass();
                    if (unit.EditorData.Group == null)
                    {
                        //assign default group to things without them.
                        unit.EditorData.Group = new List<string>();
                        string[] parsedName = unit.Name.Split("_");

                        for (int i = 0; i < parsedName.Length - 1; i++)
                        {
                            string group = parsedName[i];
                            if (i + 1 < parsedName.Length)
                            {
                                int ext;
                                bool nextNumerical = int.TryParse(parsedName[i + 1], out ext);
                                if (nextNumerical)
                                {
                                    group += "_" + parsedName[i + 1];
                                    i++;
                                    break; //dont add this to the group.
                                }
                            }
                            unit.EditorData.Group.Add(group);
                        }

                    }

                    if (!Units.ContainsKey(unit.Name))
                    {
                        Units.Add(unit.Name, unit);
                    }
                    else
                    {
                        Units[unit.Name] = unit;
                    }
                }
            }
            UpdateModule();
        }
        public void SetSelectedUnit(string name)
        {
            if (Units.ContainsKey(name))
            {
                SelectedUnit = Units[name];
            }
            else
            {
                SelectedUnit = null;
            }

            SelectedUnitChangedArgs args = new SelectedUnitChangedArgs()
            {
                SelectedUnit = SelectedUnit
            };
            SelectedUnitChanged?.Invoke(this, args);
        }
        public class SelectedUnitChangedArgs
        {
            public Unit SelectedUnit { get; set; }
        }
        public event EventHandler<SelectedUnitChangedArgs> SelectedUnitChanged;
        public OperatorRegistrantToolstrip Operators_UnitRightClicked { get; private set; }
        #endregion


        #region squad
        public class Squad
        {
            [YAXAttributeForClass]
            [YAXSerializeAs("name")]
            public string Name { get; set; }

            public class UnitRef
            {

                [YAXAttributeForClass]
                [YAXSerializeAs("count")] 
                public int Count { get; set; }

                public enum RoleType
                {
                    Normal
                }
                [YAXAttributeForClass]
                [YAXSerializeAs("role")] 
                public RoleType Role { get; set; }

                public string Name { get; set; }
            }
            public List<UnitRef> Units { get; set; }
        }
        public Dictionary<string, Squad> Squads { get; set; } = new Dictionary<string, Squad>();
        #endregion
    }
}
