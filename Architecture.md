flowchart TB
  A["Read order:\n1) Containers\n2) WPF Navigation\n3) Domain\n4) States\n5) Sequences"] --> B["When lost:\nOpen 01-containers + 02-wpf-navigation"]
  B --> C["When implementing:\nUse 04-domain + 05-states as truth\nthen implement endpoints from 03-wpf-layers mapping"]
