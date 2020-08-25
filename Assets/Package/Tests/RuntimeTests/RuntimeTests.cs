using System.Collections;
using System.Collections.Generic;
using Gaze.VirtualPointer;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Tests
{
    public class RuntimeTests
    {
        [UnityTest]
        public IEnumerator PointerHierarchy()
        {
            var pointer = new GameObject().AddComponent<VirtualPointer>();
            LogAssert.Expect(LogType.Error, "Pointer not within a canvas!");

            yield return null;
            Assert.IsTrue(pointer == null);

            var canvas = new GameObject().AddComponent<Canvas>().transform;
            var pointerGO = new GameObject();
            pointerGO.transform.SetParent(canvas);
            pointer = pointerGO.AddComponent<VirtualPointer>();

            yield return null;
            Assert.IsFalse(pointer == null);

            Object.Destroy(canvas.gameObject);
        }

        [UnityTest]
        public IEnumerator PointerMove()
        {
            var canvas = new GameObject().AddComponent<Canvas>().transform;
            var pointerGO = new GameObject();
            pointerGO.transform.SetParent(canvas);
            var pointer = pointerGO.AddComponent<VirtualPointer>();

            yield return null;
            TestPointerMove(pointer);

            Object.Destroy(canvas.gameObject);
        }

        [UnityTest]
        public IEnumerator PointerSetPosition()
        {
            var canvas = new GameObject().AddComponent<Canvas>().transform;
            var pointerGO = new GameObject();
            pointerGO.transform.SetParent(canvas);
            var pointer = pointerGO.AddComponent<VirtualPointer>();

            yield return null;
            TestPointerSetPosition(pointer);

            Object.Destroy(canvas.gameObject);
        }

        [UnityTest]
        public IEnumerator PointerMixedMove()
        {
            var canvas = new GameObject().AddComponent<Canvas>().transform;
            var pointerGO = new GameObject();
            pointerGO.transform.SetParent(canvas);
            var pointer = pointerGO.AddComponent<VirtualPointer>();

            yield return null;
            TestPointerMove(pointer);
            TestPointerSetPosition(pointer);
            TestPointerMove(pointer);
            TestPointerSetPosition(pointer);

            Object.Destroy(canvas.gameObject);
        }

        [UnityTest]
        public IEnumerator PointerClick()
        {
            var canvas = new GameObject().AddComponent<Canvas>().transform;
            canvas.gameObject.AddComponent<GraphicRaycaster>();
            var pointerGO = new GameObject();
            pointerGO.transform.SetParent(canvas);
            var pointer = pointerGO.AddComponent<VirtualPointer>();

            yield return null;

            int testVal = 0;

            var newButton = new GameObject().AddComponent<Button>();
            newButton.transform.SetParent(canvas);
            newButton.onClick.AddListener(() => ClickAddTest(ref testVal));

            newButton.gameObject.AddComponent<RectTransform>();
            newButton.gameObject.AddComponent<Image>();

            newButton.transform.position = Vector3.zero;

            for (int n = 0; n < 50; n++)
            {
                pointer.SetPointerPosition(newButton.transform.position);
                pointer.PointerDown = true;
                yield return null;
                pointer.PointerDown = false;
                pointer.SetPointerPosition(new Vector3(999, 999, 0));
                pointer.PointerDown = true;
                yield return null;
                pointer.PointerDown = false;
                yield return null;
            }

            Assert.AreEqual(49, testVal);

            Object.Destroy(canvas.gameObject);
        }

        [UnityTest]
        public IEnumerator Drag()
        {
            var canvas = new GameObject().AddComponent<Canvas>().transform;
            canvas.gameObject.AddComponent<GraphicRaycaster>();
            var pointerGO = new GameObject();
            pointerGO.transform.SetParent(canvas);
            var pointer = pointerGO.AddComponent<VirtualPointer>();

            yield return null;

            //Procedural scrollbar
            var newScrollBar = new GameObject().AddComponent<Scrollbar>();
            newScrollBar.transform.SetParent(canvas);
            var rectTransform = newScrollBar.GetComponent<RectTransform>();
            rectTransform.pivot = Vector2.zero;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.sizeDelta = new Vector2(100, 10);
            rectTransform.position = Vector3.zero;
            rectTransform.gameObject.AddComponent<Image>();

            var handle = new GameObject().AddComponent<RectTransform>();
            handle.transform.SetParent(rectTransform);
            handle.sizeDelta = Vector2.zero;
            //handle.gameObject.AddComponent<Image>().color = Color.red;

            newScrollBar.handleRect = handle;

            float result = 0;

            newScrollBar.onValueChanged.AddListener((value) => result = value);
            //

            pointer.SetPointerPosition(new Vector3(0, 5, 0));
            yield return null;
            pointer.PointerDown = true;
            yield return new WaitForSeconds(0.2f);
            pointer.PointerDown = false;
            yield return null;

            Assert.IsTrue(Mathf.Approximately(result, 0));

            pointer.SetPointerPosition(new Vector3(20, 5, 0));
            yield return null;
            pointer.PointerDown = true;
            yield return new WaitForSeconds(0.2f);
            pointer.PointerDown = false;
            yield return null;

            Assert.IsTrue(Mathf.Approximately(result, 0.125f));

            pointer.SetPointerPosition(new Vector3(40, 5, 0));
            yield return null;
            pointer.PointerDown = true;
            yield return new WaitForSeconds(0.2f);
            pointer.PointerDown = false;
            yield return null;

            Assert.IsTrue(Mathf.Approximately(result, 0.375f));

            pointer.SetPointerPosition(new Vector3(60, 5, 0));
            yield return null;
            pointer.PointerDown = true;
            yield return new WaitForSeconds(0.2f);
            pointer.PointerDown = false;
            yield return null;

            Assert.IsTrue(Mathf.Approximately(result, 0.5f));

            pointer.SetPointerPosition(new Vector3(80, 5, 0));
            yield return null;
            pointer.PointerDown = true;
            yield return new WaitForSeconds(0.2f);
            pointer.PointerDown = false;
            yield return null;

            Assert.IsTrue(Mathf.Approximately(result, 0.875f));

            pointer.SetPointerPosition(new Vector3(100, 5, 0));
            yield return null;
            pointer.PointerDown = true;
            yield return new WaitForSeconds(0.2f);
            pointer.PointerDown = false;
            yield return null;

            Assert.IsTrue(Mathf.Approximately(result, 1));

            Object.Destroy(canvas.gameObject);
        }

        private void TestPointerMove(VirtualPointer pointer)
        {
            pointer.transform.position = Vector3.zero;
            Assert.IsTrue(PointerMoveTest(Vector2.zero, pointer));
            Assert.IsTrue(PointerMoveTest(Vector2.up, pointer));
            Assert.IsTrue(PointerMoveTest(Vector2.down, pointer));
            Assert.IsTrue(PointerMoveTest(Vector2.right, pointer));
            Assert.IsTrue(PointerMoveTest(Vector2.left, pointer));
        }

        private void TestPointerSetPosition(VirtualPointer pointer)
        {
            for (int i = 0; i < 100; i++)
            {
                Assert.IsTrue(PointerSetPositionTest(GetRandomPositionWithinBounds(), pointer));
            }
            Assert.IsTrue(PointerSetPositionTest(Vector3.zero, pointer));
        }

        private bool PointerMoveTest(Vector2 movement, VirtualPointer pointer)
        {
            var initialPosition = pointer.transform.position;
            pointer.MovePointer(movement);
            return (pointer.transform.position == initialPosition + (Vector3)movement);
        }

        private bool PointerSetPositionTest(Vector3 position, VirtualPointer pointer)
        {
            pointer.SetPointerPosition(position);
            return (pointer.transform.position == position);
        }

        private Vector3 GetRandomPositionWithinBounds()
        {
            return new Vector3(
                Random.Range(0, Screen.width),
                Random.Range(0, Screen.height),
                0
                );
        }

        private void ClickAddTest(ref int testInt)
        {
            testInt += 1;
        }
    }
}
