#!/bin/bash
rm -rd events
mono cashbook.console.exe deposit -d:1.1.2014 -a:1000 -f
mono cashbook.console.exe withdraw -d:2.1.2014 -a:15 -desc:taxi -f
mono cashbook.console.exe withdraw -d:10.1.2014 -a:20 -desc:paper -f
mono cashbook.console.exe deposit -d:15.1.2014 -a:50 -desc:refund -f
mono cashbook.console.exe withdraw -d:10.2.2014 -a:65 -desc:"business cards" -f
mono cashbook.console.exe withdraw -d:17.2.2014 -a:10 -desc:pens -f
mono cashbook.console.exe withdraw -d:31.12.2013 -a:15 -desc:snacks
mono cashbook.console.exe withdraw -d:31.12.2100 -a:15 -desc:snacks
mono cashbook.console.exe sheet -m:1.1.2014
mono cashbook.console.exe sheet -m:1.2.2014
mono cashbook.console.exe deposit -d:1.12.2013 -a:100 -f
mono cashbook.console.exe sheet -m:1.1.2014