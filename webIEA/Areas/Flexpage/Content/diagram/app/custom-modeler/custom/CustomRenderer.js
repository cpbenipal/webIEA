import inherits from 'inherits';

import {
  isObject,
  assign,
  forEach
} from 'min-dash';

import BaseRenderer from 'diagram-js/lib/draw/BaseRenderer';

import {
  componentsToPath,
  createLine
} from 'diagram-js/lib/util/RenderUtil';

import {
  isTypedEvent,
  isThrowEvent,
  isCollection,
  getDi,
  getSemantic,
  getCirclePath,
  getRoundRectPath,
  getDiamondPath,
  getRectPath,
  getFillColor,
  getStrokeColor
} from 'bpmn-js/lib/draw/BpmnRenderUtil';

import {
  query as domQuery
} from 'min-dom';

import {
  is,
  getBusinessObject
} from 'bpmn-js/lib/util/ModelUtil';
import {
  append as svgAppend,
  attr as svgAttr,
  create as svgCreate,
  classes as svgClasses
} from 'tiny-svg';

var COLOR_GREEN = '#52B415',
    COLOR_RED = '#cc0000',
    COLOR_YELLOW = '#ffc800';
    import Ids from 'ids';
    var RENDERER_IDS = new Ids();

var TASK_BORDER_RADIUS = 0;
var INNER_OUTER_DIST = 3;

var DEFAULT_FILL_OPACITY = .95,
    HIGH_FILL_OPACITY = .35;

/**
 * A renderer that knows how to render custom elements.
 */

export default function CustomRenderer(
  config, eventBus, styles, pathMap,
  canvas, textRenderer, priority) {
  this.defaultFillColor = config && config.defaultFillColor,
  this.defaultStrokeColor = '#0072C6';
  this.defaultStrokeColorText = 'black';
  BaseRenderer.call(this, eventBus, 2000);
  var rendererId = RENDERER_IDS.next();
  var computeStyle = styles.computeStyle;

  this.drawRect=function(parentGfx, width, height, r, offset, attrs) {

    if (isObject(offset)) {
      attrs = offset;
      offset = 0;
    }

    offset = offset || 0;

    attrs = computeStyle(attrs, {
      stroke: 'black',
      strokeWidth: 3,
      fill: 'white'
    });

    var rect = svgCreate('rect');
    svgAttr(rect, {
      x: offset,
      y: offset,
      width: width - offset * 2,
      height: height - offset * 2,
      rx: r,
      ry: r
    });
    svgAttr(rect, attrs);

    svgAppend(parentGfx, rect);

    return rect;
  }

  this.drawCircle=function (parentGfx, width, height, offset, attrs) {

    if (isObject(offset)) {
      attrs = offset;
      offset = 0;
    }

    offset = offset || 0;

    attrs = computeStyle(attrs, {
      stroke: 'black',
      strokeWidth: 2,
      fill: 'white'
    });

    if (attrs.fill === 'none') {
      delete attrs.fillOpacity;
    }

    var cx = width / 2,
        cy = height / 2;

    var circle = svgCreate('circle');
    svgAttr(circle, {
      cx: cx,
      cy: cy,
      r: Math.round((width + height) / 4 - offset)
    });
    svgAttr(circle, attrs);

    svgAppend(parentGfx, circle);

    return circle;
  };

  this.getCirclePath = function(shape) {
    var cx = shape.x + shape.width / 2,
        cy = shape.y + shape.height / 2,
        radius = shape.width / 2;

    var circlePath = [
      ['M', cx, cy],
      ['m', 0, -radius],
      ['a', radius, radius, 0, 1, 1, 0, 2 * radius],
      ['a', radius, radius, 0, 1, 1, 0, -2 * radius],
      ['z']
    ];

    return componentsToPath(circlePath);
  };
  this.renderLabel= function(parentGfx, label, options) {

    options = assign({
      size: {
        width: 100
      }
    }, options);
    this.drawRect(parentGfx, options.box.width, 24, TASK_BORDER_RADIUS, {
      strokeWidth: 0,
      fill:this.defaultStrokeColor
    });
    var text = textRenderer.createText(label || '', options);

    svgClasses(text).add('djs-label');

    svgAppend(parentGfx, text);

    return text;
  }
  
  this.renderEmbeddedLabel =function(parentGfx, element, align) {
    var semantic = getSemantic(element);

    return this.renderLabel(parentGfx, semantic.name, {
      box: element,
      align: align,
      padding: 5,
      style: {
        fill: getStrokeColor(element, 'white')
      }
    });
  }
  
  this.attachTaskMarkers= function(parentGfx, element, taskMarkers) {
    var obj = getSemantic(element);

    var subprocess = taskMarkers && taskMarkers.indexOf('SubProcessMarker') !== -1;
    var position;

    if (subprocess) {
      position = {
        seq: -21,
        parallel: -22,
        compensation: -42,
        loop: -18,
        adhoc: 10
      };
    } else {
      position = {
        seq: -3,
        parallel: -6,
        compensation: -27,
        loop: 0,
        adhoc: 10
      };
    }

    
  }
  this.drawPath=function(parentGfx, d, attrs) {

    attrs = computeStyle(attrs, [ 'no-fill' ], {
      strokeWidth: 2,
      stroke: 'black'
    });

    var path = svgCreate('path');
    svgAttr(path, { d: d });
    svgAttr(path, attrs);

    svgAppend(parentGfx, path);

    return path;
  }
  var markers = {};
  function  addMarker(id, options) {
    var attrs = assign({
      fill: 'black',
      strokeWidth: 1,
      strokeLinecap: 'round',
      strokeDasharray: 'none'
    }, options.attrs);

    var ref = options.ref || { x: 0, y: 0 };

    var scale = options.scale || 1;

    // fix for safari / chrome / firefox bug not correctly
    // resetting stroke dash array
    if (attrs.strokeDasharray === 'none') {
      attrs.strokeDasharray = [10000, 1];
    }

    var marker = svgCreate('marker');

    svgAttr(options.element, attrs);

    svgAppend(marker, options.element);

    svgAttr(marker, {
      id: id,
      viewBox: '0 0 20 20',
      refX: ref.x,
      refY: ref.y,
      markerWidth: 20 * scale,
      markerHeight: 20 * scale,
      orient: 'auto'
    });

    var defs = domQuery('defs', canvas._svg);

    if (!defs) {
      defs = svgCreate('defs');

      svgAppend(canvas._svg, defs);
    }

    svgAppend(defs, marker);

    markers[id] = marker;
  }
  
  function marker(type, fill, stroke) {
    var id = type + '-' + colorEscape(fill) + '-' + colorEscape(stroke) + '-' + rendererId;

    if (!markers[id]) {
      createMarker(id, type, fill, stroke);
    }

    return 'url(#' + id + ')';
  }

  function createMarker(id, type, fill, stroke) {

    if (type === 'sequenceflow-end') {
      var sequenceflowEnd = svgCreate('path');
      svgAttr(sequenceflowEnd, { d: 'M 1 5 L 11 10 L 1 15 Z' });

      addMarker(id, {
        element: sequenceflowEnd,
        ref: { x: 11, y: 10 },
        scale: 0.5,
        attrs: {
          fill: stroke,
          stroke: stroke
        }
      });
    }

    if (type === 'messageflow-start') {
      var messageflowStart = svgCreate('circle');
      svgAttr(messageflowStart, { cx: 6, cy: 6, r: 3.5 });

      addMarker(id, {
        element: messageflowStart,
        attrs: {
          fill: fill,
          stroke: stroke
        },
        ref: { x: 6, y: 6 }
      });
    }

    if (type === 'messageflow-end') {
      var messageflowEnd = svgCreate('path');
      svgAttr(messageflowEnd, { d: 'm 1 5 l 0 -3 l 7 3 l -7 3 z' });

      addMarker(id, {
        element: messageflowEnd,
        attrs: {
          fill: fill,
          stroke: stroke,
          strokeLinecap: 'butt'
        },
        ref: { x: 8.5, y: 5 }
      });
    }

    if (type === 'association-start') {
      var associationStart = svgCreate('path');
      svgAttr(associationStart, { d: 'M 11 5 L 1 10 L 11 15' });

      addMarker(id, {
        element: associationStart,
        attrs: {
          fill: 'none',
          stroke: stroke,
          strokeWidth: 1.5
        },
        ref: { x: 1, y: 10 },
        scale: 0.5
      });
    }

    if (type === 'association-end') {
      var associationEnd = svgCreate('path');
      svgAttr(associationEnd, { d: 'M 1 5 L 11 10 L 1 15' });

      addMarker(id, {
        element: associationEnd,
        attrs: {
          fill: 'none',
          stroke: stroke,
          strokeWidth: 1.5
        },
        ref: { x: 12, y: 10 },
        scale: 0.5
      });
    }

    if (type === 'conditional-flow-marker') {
      var conditionalflowMarker = svgCreate('path');
      svgAttr(conditionalflowMarker, { d: 'M 0 10 L 8 6 L 16 10 L 8 14 Z' });

      addMarker(id, {
        element: conditionalflowMarker,
        attrs: {
          fill: fill,
          stroke: stroke
        },
        ref: { x: -1, y: 10 },
        scale: 0.5
      });
    }

    if (type === 'conditional-default-flow-marker') {
      var conditionaldefaultflowMarker = svgCreate('path');
      svgAttr(conditionaldefaultflowMarker, { d: 'M 6 4 L 10 16' });

      addMarker(id, {
        element: conditionaldefaultflowMarker,
        attrs: {
          stroke: stroke
        },
        ref: { x: 0, y: 10 },
        scale: 0.5
      });
    }
  }
  function colorEscape(str) {
    return str.replace(/[()\s,#]+/g, '_');
  }
  this.createPathFromConnection=function(connection) {
    var waypoints = connection.waypoints;

    var pathData = 'm  ' + waypoints[0].x + ',' + waypoints[0].y;
    for (var i = 1; i < waypoints.length; i++) {
      pathData += 'L' + waypoints[i].x + ',' + waypoints[i].y + ' ';
    }
    return pathData;
  }
  var that=this;
  
  function as(type) {
    return function(parentGfx, element) {
    
     
      var result=that.handlers[type](parentGfx, element);
     
      return result;
    };
  }
  this.handlers  = {
    'custom:operation': function(parentGfx, element,attrs) {
      var attrs = {
        fill: getFillColor(element, that.defaultFillColor),
        stroke: getStrokeColor(element, that.defaultStrokeColor)
      };;
  
      if (!('fillOpacity' in attrs)) {
        attrs.fillOpacity = DEFAULT_FILL_OPACITY;
      }
  
      var task = that.drawCircle(parentGfx, element.width, element.height, attrs);
      // that.renderEmbeddedLabel(parentGfx, element, 'top-left');
      // that.attachTaskMarkers(parentGfx, element);
  
      // if(element.title){
      //  element.businessObject.name=element.title;
      //  that.renderEmbeddedLabel(parentGfx, element, 'top-left');
      //  that.attachTaskMarkers(parentGfx, element);
      
      // }
      return task;
    },
    // 'custom:AND-operation': as('custom:operation'),
    // 'custom:OR-operation': as('custom:operation'),
    // 'custom:NOT-operation': as('custom:operation'),
    'bpmn:ServiceTask': as('custom:operation'),
    'custom:condition': function(parentGfx, element,attrs) {
      var attrs = {
        fill: getFillColor(element, that.defaultFillColor),
        stroke: getStrokeColor(element, that.defaultStrokeColor),
      };
  
      var rect = that.drawRect(parentGfx, element.width, element.height, TASK_BORDER_RADIUS, attrs);
  
      that.renderEmbeddedLabel(parentGfx, element, 'top-left');
      that.attachTaskMarkers(parentGfx, element);
  
      return rect;
    },
    'bpmn:Task': as('custom:condition'),
    'bpmn:UserTask': as('custom:condition'),
    'bpmn:ManualTask': as('custom:condition'),
    'custom:link': as('custom:condition'),
    'bpmn:SequenceFlow': function(parentGfx, element) {
      var pathData = that.createPathFromConnection(element);

      var fill = getFillColor(element, that.defaultFillColor),
          stroke = getStrokeColor(element, that.defaultStrokeColor);

      var attrs = {
        strokeLinejoin: 'round',
        markerEnd: marker('sequenceflow-end', fill, stroke),
        stroke: getStrokeColor(element, that.defaultStrokeColor)
      };

      var path = that.drawPath(parentGfx, pathData, attrs);

      var sequenceFlow = getSemantic(element);

      var source;

      if (element.source) {
        source = element.source.businessObject;

        // conditional flow marker
        if (sequenceFlow.conditionExpression && source.$instanceOf('bpmn:Activity')) {
          svgAttr(path, {
            markerStart: marker('conditional-flow-marker', fill, stroke)
          });
        }

        // default marker
        if (source.default && (source.$instanceOf('bpmn:Gateway') || source.$instanceOf('bpmn:Activity')) &&
            source.default === sequenceFlow) {
          svgAttr(path, {
            markerStart: marker('conditional-default-flow-marker', fill, stroke)
          });
        }
      }

      return path;
    },
    'connection': as('bpmn:SequenceFlow'),
    
  }
}

inherits(CustomRenderer, BaseRenderer);

CustomRenderer.$inject = [
  'config.bpmnRenderer',
  'eventBus',
  'styles',
  'pathMap',
  'canvas',
  'textRenderer' ];


CustomRenderer.prototype.canRender = function(element) {
  var type = element.type;
  var canRender=type!="label";
  
  return canRender;
};
function getType(element){
  var elems= element.parent.businessObject.flowElements;
   var select=elems.find(e=>e.id=='Task_select');
   var type=select.$attrs["bpmn:sField"]||select.$attrs["sField"];
   if(type=='Persons'){
     type='Person';
   }else if(type=='Companies'){
     type='Company';
   }
   
   if(element.id!='Task_select'){
    var link=getLink(element,elems.filter(e=>e.$type=='bpmn:ManualTask'),element.id);
    if(link){
        type=link.$attrs["bpmn:sField"]||link.$attrs["sField"];
    }
  }
   return type;
 }
 function getLink(element,elems,id){
  var link=elems.find(el=>{
    if(el.outgoing&&(el.$type=='bpmn:ManualTask'||el.$type=='bpmn:ServiceTask')){
      for(var i=0;i<el.outgoing.length;i++){
        var outgoing=el.outgoing[i].targetRef;
        if(el.$type=='bpmn:ManualTask'&&outgoing.$type=="bpmn:UserTask"&&id==outgoing.id){
          return el;
        }else if(id==outgoing.id){
          return true;
        }else if(el.$type=='bpmn:ManualTask'&&outgoing.$type=="bpmn:ServiceTask"){
          if(getLink(outgoing,element.parent.businessObject.flowElements.filter(e=>e.id!=el.id),id)){
           return el;
          }
        }else if(el.$type=='bpmn:ServiceTask'&&outgoing.$type=="bpmn:ServiceTask"){
          return getLink(outgoing,element.parent.businessObject.flowElements.filter(e=>e.id!=el.id),id);
        }
      }
    }
    return false;
  });
  return link;
 }
CustomRenderer.prototype.drawShape = function(parentGfx, element) {
  var type = element.type;
  var h = this.handlers[type];
 if(element.businessObject&&element.parent){
  if(element.businessObject.$type=='bpmn:UserTask'){
    var type=getType(element);
      element.businessObject.$attrs["sType"]=type;
    element.businessObject.name='Filter on a '+type+' field';
    element.businessObject.title='Filter on a '+type+' field';
  }
  if(element.businessObject.$type=='bpmn:ManualTask'){
    if(!element.businessObject.$attrs["sType"]){
      element.businessObject.$attrs["sType"]='(any)';
      element.businessObject.$attrs["sField"]='Person';
    }
  }
  if(element.businessObject.$type=='bpmn:ServiceTask'&&!element.businessObject.$attrs["sField"]){
    element.businessObject.$attrs["sField"]='OR';
  }
  if(element.businessObject.$type=='bpmn:ManualTask'){
    element.businessObject.name='Consider linked data';
    element.businessObject.title='Consider linked data';
  }
}
  /* jshint -W040 */
  return h(parentGfx, element);
};

CustomRenderer.prototype.getShapePath = function(element) {

  if (is(element, 'bpmn:ServiceTask')) {
    return getCirclePath(element);
  }

  if (is(element, 'bpmn:Activity')) {
    return getRoundRectPath(element, TASK_BORDER_RADIUS);
  }

  if (is(element, 'bpmn:Gateway')) {
    return getDiamondPath(element);
  }

  return getRectPath(element);
};

CustomRenderer.prototype.drawConnection = function(parentGfx, element) {

  var type = element.type;
  var h = this.handlers[type];

  /* jshint -W040 */
  return h(parentGfx, element);
};


CustomRenderer.prototype.getConnectionPath = function(connection) {

  var type = connection.type;

    return this.createPathFromConnection(connection);
};
