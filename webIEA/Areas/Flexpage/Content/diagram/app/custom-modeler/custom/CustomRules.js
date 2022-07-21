import {
  every,
  find,
  forEach,
  some
} from 'min-dash';

import inherits from 'inherits';

import {
  is,
  getBusinessObject
} from 'bpmn-js/lib/util/ModelUtil';

import {
  getParent,
  isAny
} from 'bpmn-js/lib/features/modeling/util/ModelingUtil';

import {
  isLabel
} from 'bpmn-js/lib/util/LabelUtil';

import {
  isExpanded,
  isEventSubProcess,
  isInterrupting,
  hasErrorEventDefinition,
  hasEscalationEventDefinition,
  hasCompensateEventDefinition
} from 'bpmn-js/lib/util/DiUtil';

import {
  getBoundaryAttachment as isBoundaryAttachment
} from 'bpmn-js/lib/features/snapping/BpmnSnappingUtil';


import BpmnRules from 'bpmn-js/lib/features/rules/BpmnRules';
var HIGH_PRIORITY = 1500;


/**
 * Specific rules for custom elements
 */
export default function CustomRules(eventBus,canvas) {
  this.eventBus=eventBus;
  this.canvas=canvas;
  BpmnRules.call(this, eventBus);
}

CustomRules.$inject = [ 'eventBus','canvas' ];

inherits(CustomRules, BpmnRules);




CustomRules.prototype.init = function() {
 var that=this;
  
 this.addRule('connection.start', HIGH_PRIORITY, function(context) {
  if(context.source!=null&&context.source.type!="bpmn:Process"){
  var isSelect=context.source.type=='bpmn:Task'&&context.source.id=='Task_select';
  var isCondition=context.source.type=='bpmn:UserTask';
  var isNOTOperation=context.source.type=='bpmn:ServiceTask'&&context.source.businessObject&&context.source.businessObject.$attrs["sField"]=="NOT";
  var isLink=context.source.type=='bpmn:ManualTask';
  var isArrow=context.source.type=='bpmn:SequenceFlow';
  var hasOneOutgoing=context.source.outgoing&&context.source.outgoing.length>0||
  context.source.businessObject&&context.source.businessObject.outgoing&&context.source.businessObject.outgoing.length>0;
  var allowed = 
  !(isSelect && hasOneOutgoing)&&
  !isCondition &&
  !(isLink && hasOneOutgoing)&&
  !(isNOTOperation && hasOneOutgoing)&&
  !isArrow;

  return allowed;
  }
  return null;
});
this.addRule('connection.create', HIGH_PRIORITY, function (context) {
  if(context.target!=null&&context.target.type!="bpmn:Process"){
    var isSelect=context.target.type=='bpmn:Task'&&context.target.id=='Task_select';
    var isCondition=context.target.type=='bpmn:UserTask';
    var isOperation=context.target.type=='bpmn:ServiceTask';
    var isLink=context.target.type=='bpmn:ManualTask';
    var isArrow=context.target.type=='bpmn:SequenceFlow';
    var hasOneIncoming=context.target.incoming&&context.target.incoming.length>0||
    context.target.businessObject&&context.target.businessObject.incoming&&context.target.businessObject.incoming.length>0;
    var allowed =
    !isSelect&&
    !(isCondition&&hasOneIncoming) &&
    !(isLink && hasOneIncoming)&&
    !(isOperation && hasOneIncoming)&&
    context.target.id!=context.source.id;
    //return allowed;
    if(allowed){
      return { type: 'bpmn:SequenceFlow' };
    }else{
      return false;
    }
  }
  return null;
});

  
};

BpmnRules.prototype.canConnectSequenceFlow = canConnectSequenceFlow;

BpmnRules.prototype.canConnect = canConnect;



function nonExistingOrLabel(element) {
  return !element || isLabel(element);
}


function getOrganizationalParent(element) {

  do {
    if (is(element, 'bpmn:Process')) {
      return getBusinessObject(element);
    }

    if (is(element, 'bpmn:Participant')) {
      return (
        getBusinessObject(element).processRef ||
        getBusinessObject(element)
      );
    }
  } while ((element = element.parent));

}

function isTextAnnotation(element) {
  return is(element, 'bpmn:TextAnnotation');
}


function isCompensationBoundary(element) {
  return is(element, 'bpmn:BoundaryEvent') &&
         hasEventDefinition(element, 'bpmn:CompensateEventDefinition');
}

function isForCompensation(e) {
  return getBusinessObject(e).isForCompensation;
}

function isSameOrganization(a, b) {
  var parentA = getOrganizationalParent(a),
      parentB = getOrganizationalParent(b);

  return parentA === parentB;
}

function isMessageFlowSource(element) {
  return (
    is(element, 'bpmn:InteractionNode') && (
      !is(element, 'bpmn:Event') || (
        is(element, 'bpmn:ThrowEvent') &&
        hasEventDefinitionOrNone(element, 'bpmn:MessageEventDefinition')
      )
    )
  );
}

function isMessageFlowTarget(element) {
  return (
    is(element, 'bpmn:InteractionNode') &&
    !isForCompensation(element) && (
      !is(element, 'bpmn:Event') || (
        is(element, 'bpmn:CatchEvent') &&
        hasEventDefinitionOrNone(element, 'bpmn:MessageEventDefinition')
      )
    )
  );
}

function getScopeParent(element) {

  var parent = element;

  while ((parent = parent.parent)) {

    if (is(parent, 'bpmn:FlowElementsContainer')) {
      return getBusinessObject(parent);
    }

    if (is(parent, 'bpmn:Participant')) {
      return getBusinessObject(parent).processRef;
    }
  }

  return null;
}

function isSameScope(a, b) {
  var scopeParentA = getScopeParent(a),
      scopeParentB = getScopeParent(b);

  return scopeParentA === scopeParentB;
}

function hasEventDefinition(element, eventDefinition) {
  var bo = getBusinessObject(element);

  return !!find(bo.eventDefinitions || [], function(definition) {
    return is(definition, eventDefinition);
  });
}

function hasEventDefinitionOrNone(element, eventDefinition) {
  var bo = getBusinessObject(element);

  return (bo.eventDefinitions || []).every(function(definition) {
    return is(definition, eventDefinition);
  });
}

function isSequenceFlowSource(element) {
  return (
    is(element, 'bpmn:FlowNode') &&
    !is(element, 'bpmn:EndEvent') &&
    !isEventSubProcess(element) &&
    !(is(element, 'bpmn:IntermediateThrowEvent') &&
      hasEventDefinition(element, 'bpmn:LinkEventDefinition')
    ) &&
    !isCompensationBoundary(element) &&
    !isForCompensation(element)
  );
}

function isSequenceFlowTarget(element) {
  return (
    is(element, 'bpmn:FlowNode') &&
    !is(element, 'bpmn:StartEvent') &&
    !is(element, 'bpmn:BoundaryEvent') &&
    !isEventSubProcess(element) &&
    !(is(element, 'bpmn:IntermediateCatchEvent') &&
      hasEventDefinition(element, 'bpmn:LinkEventDefinition')
    ) &&
    !isForCompensation(element)
  );
}

function isEventBasedTarget(element) {
  return (
    is(element, 'bpmn:ReceiveTask') || (
      is(element, 'bpmn:IntermediateCatchEvent') && (
        hasEventDefinition(element, 'bpmn:MessageEventDefinition') ||
        hasEventDefinition(element, 'bpmn:TimerEventDefinition') ||
        hasEventDefinition(element, 'bpmn:ConditionalEventDefinition') ||
        hasEventDefinition(element, 'bpmn:SignalEventDefinition')
      )
    )
  );
}

function isConnection(element) {
  return element.waypoints;
}

function getParents(element) {

  var parents = [];

  while (element) {
    element = element.parent;

    if (element) {
      parents.push(element);
    }
  }

  return parents;
}

function isParent(possibleParent, element) {
  var allParents = getParents(element);
  return allParents.indexOf(possibleParent) !== -1;
}

function canConnect(source, target, connection) {

  if (nonExistingOrLabel(source) || nonExistingOrLabel(target)) {
    return null;
  }

  if (!is(connection, 'bpmn:DataAssociation')) {

    if (canConnectMessageFlow(source, target)) {
      return { type: 'bpmn:MessageFlow' };
    }

    if (canConnectSequenceFlow(source, target)) {
      return { type: 'bpmn:SequenceFlow' };
    }
  }

  var connectDataAssociation = canConnectDataAssociation(source, target);

  if (connectDataAssociation) {
    return connectDataAssociation;
  }

  if (isCompensationBoundary(source) && isForCompensation(target)) {
    return {
      type: 'bpmn:Association',
      associationDirection: 'One'
    };
  }

  if (canConnectAssociation(source, target)) {

    return {
      type: 'bpmn:Association'
    };
  }

  return false;
}

/**
 * Check, whether one side of the relationship
 * is a text annotation.
 */
function isOneTextAnnotation(source, target) {

  var sourceTextAnnotation = isTextAnnotation(source),
      targetTextAnnotation = isTextAnnotation(target);

  return (
    (sourceTextAnnotation || targetTextAnnotation) &&
    (sourceTextAnnotation !== targetTextAnnotation)
  );
}


function canConnectAssociation(source, target) {

  // do not connect connections
  if (isConnection(source) || isConnection(target)) {
    return false;
  }

  // compensation boundary events are exception
  if (isCompensationBoundary(source) && isForCompensation(target)) {
    return true;
  }

  // don't connect parent <-> child
  if (isParent(target, source) || isParent(source, target)) {
    return false;
  }

  // allow connection of associations between <!TextAnnotation> and <TextAnnotation>
  return isOneTextAnnotation(source, target);
}

function canConnectMessageFlow(source, target) {

  // during connect user might move mouse out of canvas
  // https://github.com/bpmn-io/bpmn-js/issues/1033
  if (getRootElement(source) && !getRootElement(target)) {
    return false;
  }

  return (
    isMessageFlowSource(source) &&
    isMessageFlowTarget(target) &&
    !isSameOrganization(source, target)
  );
}

function canConnectSequenceFlow(source, target) {

  if (
    isEventBasedTarget(target) &&
    target.incoming.length > 0 &&
    areOutgoingEventBasedGatewayConnections(target.incoming) &&
    !is(source, 'bpmn:EventBasedGateway')
  ) {
    return false;
  }

  if(source.type=="bpmn:UserTask"){
    return false;
  }
  return isSequenceFlowSource(source) &&
         isSequenceFlowTarget(target) &&
         isSameScope(source, target) &&
         !(is(source, 'bpmn:EventBasedGateway') && !isEventBasedTarget(target));
}


function canConnectDataAssociation(source, target) {

  if (isAny(source, [ 'bpmn:DataObjectReference', 'bpmn:DataStoreReference' ]) &&
      isAny(target, [ 'bpmn:Activity', 'bpmn:ThrowEvent' ])) {
    return { type: 'bpmn:DataInputAssociation' };
  }

  if (isAny(target, [ 'bpmn:DataObjectReference', 'bpmn:DataStoreReference' ]) &&
      isAny(source, [ 'bpmn:Activity', 'bpmn:CatchEvent' ])) {
    return { type: 'bpmn:DataOutputAssociation' };
  }

  return false;
}






function isOutgoingEventBasedGatewayConnection(connection) {

  if (connection && connection.source) {
    return is(connection.source, 'bpmn:EventBasedGateway');
  }
}

function areOutgoingEventBasedGatewayConnections(connections) {
  connections = connections || [];

  return connections.some(isOutgoingEventBasedGatewayConnection);
}

function getRootElement(element) {
  return getParent(element, 'bpmn:Process') || getParent(element, 'bpmn:Collaboration');
}