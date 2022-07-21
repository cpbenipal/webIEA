
import {
  forEach
} from 'min-dash';
module.exports.setValues = function(element,values,settings) {
  var $ = require('jquery');
  var v=values.join(' ').trim();
  
   element.businessObject.text=v;
   element.businessObject.name=v;
   Object.keys(settings).forEach(key=>{
    key=key.replace("bpmn:","");
     if(settings[key]){
      element.businessObject.$attrs[key]=settings[key];
      element.businessObject.$attrs["bpmn:"+key]=settings[key];
    }else if(key=='sDescription'||key=='sValue'){
      element.businessObject.$attrs[key]='';
      element.businessObject.$attrs["bpmn:"+key]='';
    }
   })
   
};


