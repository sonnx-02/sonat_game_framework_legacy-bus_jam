namespace Test
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;


    public class TestPropertyDrawer : MonoBehaviour
    {
        public CharacterAnimatorController2 controller2;

        public MapTest mapTest;
        // Start is called before the first frame update

        [MyButtonIntParameterPreset(nameof(OnChanged), "", new int[] {-1, 0, 1, 2})] [SerializeField]
        private int btn2;

        [MyButtonRange(0, 10, nameof(OnChanged))]
        public int abc1;

        public void OnChanged(int a)
        {
            Debug.Log("OnChanged" + a);
        }

        [MyButtonInt(nameof(TestInt), nameof(TestFloat), nameof(TestBool))]
        public int abc;

        public void TestInt(int a)
        {
            Debug.Log(a);
        }

        public void TestFloat(float a)
        {
            Debug.Log(a);
        }

        public void TestBool(bool a)
        {
            Debug.Log(a);
        }


        public SaveLoadLevelDatabaseSample testSaveLoadLevel;

        public List<Level> levelDatabase = new List<Level>();
        public int Count => levelDatabase.Count;

        [MyButtonIntPageSelect(new[] {nameof(LoadLevel), nameof(PlayLevel), nameof(SaveLevel)},
            new[] {"Load", "Play", "Save"}, nameof(levelDatabase))]
        public int test1;

        public Level tempLevel;

        public void LoadLevel(int value)
        {
            tempLevel = levelDatabase[value].Clone();
        }

        public void PlayLevel(int value)
        {
            Debug.Log(levelDatabase[value].point.toString());
        }

        public void SaveLevel(int value)
        {
            if (value == levelDatabase.Count)
                levelDatabase.Add(tempLevel.Clone());
            else
                levelDatabase[value] = tempLevel.Clone();
        }


        [Serializable]
        public class Level
        {
            public Point point;

            public Level Clone()
            {
                return new Level()
                {
                    point = point.Clone()
                };
            }
        }
    }

    [Serializable]
    public class SaveLoadLevelDatabaseSample
    {
        public SaveLoadLevelDatabaseSample testSaveLoadLevel;

        public List<Level> levelDatabase = new List<Level>();
        public int Count => levelDatabase.Count;

        [MyButtonIntPageSelect(new[] {nameof(LoadLevel2), nameof(PlayLevel2), nameof(SaveLevel2)},
            new[] {"Load", "Play", "Save"}, nameof(levelDatabase))]
        public int test1;

        public Level tempLevel;

        public void LoadLevel2(int value)
        {
            tempLevel = levelDatabase[value].Clone();
        }

        public void PlayLevel2(int value)
        {
            Debug.Log(levelDatabase[value].point.toString());
        }

        public void SaveLevel2(int value)
        {
            if (value == levelDatabase.Count)
                levelDatabase.Add(tempLevel.Clone());
            else
                levelDatabase[value] = tempLevel.Clone();
        }


        [Serializable]
        public class Level
        {
            public Point point;

            public Level Clone()
            {
                return new Level()
                {
                    point = point.Clone()
                };
            }
        }
    }


    [Serializable]
    public class MapTest
    {
        public ListPoint listPoint;
        public ListPoint[] listPoint2;
    }

    [Serializable]
    public class CharacterAnimatorController2
    {
#if UNITY_EDITOR
        [MyButton(
            nameof(TriggerAnimatorIdle),
            nameof(TriggerAnimatorTurnLeft),
            nameof(TriggerAnimatorTurnRight)
        )]
        [SerializeField]
        private int test1;

        [MyButtonInt(
            new[]
            {
                nameof(TriggerAnimatorAttack),
                nameof(TriggerAnimatorRun),
                nameof(TriggerAnimatorDie),
                nameof(TriggerAnimatorOnHit)
            }
        )]
        [SerializeField]
        private int test2;

        [MyButtonString(
            new[]
            {
                nameof(DebugShow),
            }
        )]
        [SerializeField]
        private string test3;

        [MyButtonFloat(
            new[]
            {
                nameof(DebugShowF),
            }
        )]
        [SerializeField]
        private float test4;
#endif

        public void DebugShowF(float abc)
        {
            Debug.Log(abc);
        }

        public void DebugShow(string abc)
        {
            Debug.Log(abc);
        }

        [SerializeField] private string triggerIdle;
        [SerializeField] private string[] triggerAttack;
        [SerializeField] private string[] triggerRun;
        [SerializeField] private string[] triggerDie;
        [SerializeField] private string[] triggerOnHit;
        [SerializeField] private string triggerTurnLeft;
        [SerializeField] private string triggerTurnRight;

        private Animator _inject;

        public void Init(Animator animator)
        {
            _inject = animator;
        }

        public void TriggerAnimatorRun(int index = 0)
        {
            _inject.SetTrigger(triggerRun[index]);
        }

        public void TriggerAnimatorAttack(int index = 0)
        {
            _inject.SetTrigger(triggerAttack[index]);
        }

        public void TriggerAnimatorDie(int index = 0)
        {
            _inject.SetTrigger(triggerDie[index]);
        }

        public void TriggerAnimatorOnHit(int index = 0)
        {
            _inject.SetTrigger(triggerOnHit[index]);
        }

        public void TriggerAnimatorTurnLeft()
        {
            _inject.SetTrigger(triggerTurnLeft);
        }

        public void TriggerAnimatorTurnRight()
        {
            _inject.SetTrigger(triggerTurnRight);
        }

        public void TriggerAnimatorIdle()
        {
            _inject.SetTrigger(triggerIdle);
        }
    }
}