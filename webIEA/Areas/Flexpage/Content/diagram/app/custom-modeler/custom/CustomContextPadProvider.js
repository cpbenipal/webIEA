import inherits from 'inherits';

import ContextPadProvider from 'bpmn-js/lib/features/context-pad/ContextPadProvider';

import {
  is
} from 'bpmn-js/lib/util/ModelUtil';

import {
  isExpanded,
  isEventSubProcess
} from 'bpmn-js/lib/util/DiUtil';

import {
  isAny
} from 'bpmn-js/lib/features/modeling/util/ModelingUtil';

import {
  assign,
  forEach,
  isArray,
  bind
} from 'min-dash';


export default function CustomContextPadProvider(injector, connect, translate) {

  injector.invoke(ContextPadProvider, this);

  
  var cached = bind(this.getContextPadEntries, this);

  this.getContextPadEntries = function(element) {
    var contextPad = this._contextPad,
      modeling = this._modeling,

      elementFactory = this._elementFactory,
      connect = this._connect,
      create = this._create,
      popupMenu = this._popupMenu,
      canvas = this._canvas,
      rules = this._rules,
      autoPlace = this._autoPlace,
      translate = this._translate;

    var actions = {};

    var businessObject = element.businessObject;

    function startConnect(event, element, autoActivate) {
      connect.start(event, element, autoActivate);
    }
    function removeElement(e) {
      modeling.removeElements([ element ]);
    }
/**
   * Create an append action
   *
   * @param {String} type
   * @param {String} className
   * @param {String} [title]
   * @param {Object} [options]
   *
   * @return {Object} descriptor
   */
  function appendAction(type, className, title, options) {

    if (typeof title !== 'string') {
      options = title;
      title = translate('Append {type}', { type: type.replace(/^bpmn:/, '') });
    }

    function appendStart(event, element) {

      var shape = elementFactory.createShape(assign({ type: type,title:title }, options));
      create.start(event, shape, element);
    }


    var append = autoPlace ? function(event, element) {
      var shape = elementFactory.createShape(assign({ type: type,title:title }, options));

      autoPlace.append(element, shape);
    } : appendStart;


    return {
      group: 'model',
      className: className,
      title: title,
      action: {
        dragstart: appendStart,
        click: append
      }
    };
  }
    var isSelect=businessObject.$type=='bpmn:Task'&&businessObject.id=='Task_select';
    var hasOutgoingSelect=businessObject.outgoing&&businessObject.outgoing.length>0;
    var isCondition=businessObject.$type=='bpmn:UserTask';
    var isNOTOperation=businessObject.$type=='bpmn:ServiceTask'&&businessObject.$attrs["sField"]=="NOT";
    var hasOutgoingNOTOperation=businessObject.outgoing&&businessObject.outgoing.length>0;
    var isLink=businessObject.$type=='bpmn:ManualTask';
    var hasOutgoingLink=businessObject.outgoing&&businessObject.outgoing.length>0;
    var isArrow=businessObject.$type=='bpmn:SequenceFlow';
    if (!isCondition &&
    !(isSelect && hasOutgoingSelect)&&
    !(isLink && hasOutgoingLink)&&
    !(isNOTOperation && hasOutgoingNOTOperation)&&
    !isArrow) {
      assign(actions, {
        
        'append.OR-operation': appendAction(
          'bpmn:ServiceTask',
          'operation-image',
          translate('OR')
        ),
        'append.Condition': appendAction(
          'bpmn:UserTask',
          'condition-image',
          translate('Condition')
        ),
        'append.Link': appendAction(
          'bpmn:ManualTask',
          'link-image',
          translate('Link')
        ),
        'connect': {
          group: 'tools',
          className: 'bpmn-icon-connection',
          title: translate('Connect'),
          action: {
            click: startConnect,
            dragstart: startConnect
          }
        }
      });
    }
    
    // delete element entry, only show if allowed by rules
  var deleteAllowed = rules.allowed('elements.delete', { elements: [ element ] });
  
  deleteAllowed=deleteAllowed&&!(businessObject.$type=='bpmn:Task'&&businessObject.id=='Task_select');

  if (isArray(deleteAllowed)) {
    // was the element returned as a deletion candidate?
    deleteAllowed = deleteAllowed[0] === element;
  }

  if (deleteAllowed) {
    assign(actions, {
      'delete': {
        group: 'tools',
        className: 'bpmn-icon-trash',
        title: translate('Remove'),
        action: {
          click: removeElement
        }
      }
    });
  }
    return actions;
  };
}

function isEventType(eventBo, type, definition) {

  var isType = eventBo.$instanceOf(type);
  var isDefinition = false;

  var definitions = eventBo.eventDefinitions || [];
  forEach(definitions, function(def) {
    if (def.$type === definition) {
      isDefinition = true;
    }
  });

  return isType && isDefinition;
}
inherits(CustomContextPadProvider, ContextPadProvider);

CustomContextPadProvider.$inject = [
  'injector',
  'connect',
  'translate'
];