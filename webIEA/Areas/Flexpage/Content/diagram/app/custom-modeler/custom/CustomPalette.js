import {
  assign
} from 'min-dash';


/**
 * A palette that allows you to create BPMN _and_ custom elements.
 */
export default function PaletteProvider(palette, create, elementFactory, spaceTool, lassoTool, handTool,
  globalConnect, translate) {

  this._create = create;
  this._elementFactory = elementFactory;
  this._spaceTool = spaceTool;
  this._lassoTool = lassoTool;
  this._handTool = handTool;
  this._globalConnect = globalConnect;
  this._translate = translate;
  palette.registerProvider(this);
}

PaletteProvider.$inject = [
  'palette',
  'create',
  'elementFactory',
  'spaceTool',
  'lassoTool',
  'handTool',
  'globalConnect',
  'translate'
];


PaletteProvider.prototype.getPaletteEntries = function(element) {

  var actions  = {},
      create = this._create,
      elementFactory = this._elementFactory,
      spaceTool = this._spaceTool,
      lassoTool = this._lassoTool,
      handTool = this._handTool,
      globalConnect = this._globalConnect,
      translate = this._translate;


  function createAction(type, group, className, title, options) {

    function createListener(event) {

      var shape = elementFactory.createShape(assign({ type: type,title:title,cursor: { x: event.x+30, y: event.y }}, options));

      if (options) {
        shape.businessObject.di.isExpanded = options.isExpanded;
      }
      
      create.start(event, shape);
    }
    
    var shortType = type.replace(/^bpmn:/, '');

    return {
      group: group,
      className: className,
      title: title,
      action: {
        dragstart: createListener,
        click: createListener
      }
    };
  }

  function createParticipant(event, collapsed) {
    create.start(event, elementFactory.createParticipantShape(collapsed));
  }

  assign(actions, {
    'create.OR-operation': createAction(
      'bpmn:ServiceTask', 'event', 'operation-image',
      translate('OR')
    ),
    'create.Condition': createAction(
      'bpmn:UserTask', 'activity', 'condition-image',
      translate('Condition')
    ),
    'create.Link': createAction(
      'bpmn:ManualTask', 'activity', 'link-image',
      translate('Link')
    ),
    'hand-tool': {
      group: 'tools',
      className: 'bpmn-icon-hand-tool',
      title: translate('Hand'),
      action: {
        click: function(event) {
          handTool.activateHand(event);
        }
      }
    },
    'global-connect-tool': {
      group: 'tools',
      className: 'bpmn-icon-connection',
      title: translate('Conect'),
      action: {
        click: function(event) {
          globalConnect.toggle(event);
        }
      }
    },
  });

  return actions;
};
