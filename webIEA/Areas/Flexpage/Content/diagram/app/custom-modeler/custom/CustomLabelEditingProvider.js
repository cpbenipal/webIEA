import inherits from 'inherits';

import LabelEditingProvider from 'bpmn-js/lib/features/label-editing/LabelEditingProvider';


export default function CustomLabelEditingProvider(eventBus, bpmnFactory, canvas, directEditing, modeling, resizeHandles, textRenderer) {

  //if commented out, block editing is disabled
  //LabelEditingProvider.call(this, eventBus,bpmnFactory,canvas,directEditing,modeling,resizeHandles,textRenderer);

  
}

CustomLabelEditingProvider.$inject = [ 'eventBus', 'bpmnFactory', 'canvas', 'directEditing', 'modeling', 'resizeHandles', 'textRenderer' ];

inherits(CustomLabelEditingProvider, LabelEditingProvider);