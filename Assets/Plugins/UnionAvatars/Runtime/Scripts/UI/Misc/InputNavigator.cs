using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
 
namespace UnionAvatars.UI
{
    public class InputNavigator : MonoBehaviour
    {
        EventSystem system;
    
        void Start()
        {
            system = EventSystem.current;// EventSystemManager.currentSystem;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                SelectNextElement();
            }
        }

        public void SelectNextElement(string text)
        {
            SelectNextElement();
        }

        public void SelectNextElement()
        {
            Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
            
            if (next != null)
            {
                InputField inputfield = next.GetComponent<InputField>();
                if (inputfield != null)
                    inputfield.OnPointerClick(new PointerEventData(system));
                
                system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));
            }
        }
    }
}