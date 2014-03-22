import java.awt.Color;
import java.awt.Component;
import java.awt.Dialog;
import java.awt.Dimension;
import java.awt.FlowLayout;
import java.awt.Label;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.event.KeyEvent;
import java.awt.event.KeyListener;
import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.URI;
import java.util.HashMap;
import java.util.Iterator;
import java.util.Map;
import java.util.Set;

import javax.swing.ButtonGroup;
import javax.swing.JButton;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.swing.JRadioButton;
import javax.swing.JTextArea;
import javax.swing.JTextField;

import org.apache.http.HttpEntity;
import org.apache.http.HttpResponse;
import org.apache.http.client.ClientProtocolException;
import org.apache.http.client.HttpClient;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.impl.client.DefaultHttpClient;

/**
 * 
 * @author Aurion / aurion@gmail.com
 * 
 */
public class VindictusCraftingCostCalculator extends JFrame implements
ActionListener {

	private static final long serialVersionUID = 1L;
	static JFrame frame;
	static JTextArea display = new JTextArea();
	static String text;
	static JTextField entryField = new JTextField();
	static JPanel panel;
	public static JTextArea outputWindow = new JTextArea();
	static VindictusCraftingCostCalculator vinccc;
	FlowLayout layout = new FlowLayout(FlowLayout.LEFT);
	static int labelLength = 0;
	static boolean craftedByPlayer = false;
	static String playerCrafted = new String("Player");
	static String npcCrafted = new String("NPC");
	JRadioButton radioButtonPlayerCrafted = new JRadioButton(VindictusCraftingCostCalculator.playerCrafted);
	JRadioButton radioButtonNpcCrafted = new JRadioButton(VindictusCraftingCostCalculator.npcCrafted);

	public static void main(String[] args) {
		VindictusCraftingCostCalculator.vinccc = new VindictusCraftingCostCalculator();
		VindictusCraftingCostCalculator.vinccc.setModalExclusionType(Dialog.ModalExclusionType.APPLICATION_EXCLUDE);

	}

	public VindictusCraftingCostCalculator() {
		VindictusCraftingCostCalculator.panel = new JPanel();

		this.setAlwaysOnTop(true);
		JButton submit = new JButton("Submit");
		VindictusCraftingCostCalculator.outputWindow.setEditable(false);
		VindictusCraftingCostCalculator.outputWindow.setPreferredSize(new Dimension(300, 0));
		this.setMinimumSize(new Dimension(350, 75));

		VindictusCraftingCostCalculator.panel.add(VindictusCraftingCostCalculator.outputWindow);
		VindictusCraftingCostCalculator.entryField.setPreferredSize(new Dimension(200, 25));
		VindictusCraftingCostCalculator.entryField.addKeyListener(new KeyListener() {

			@Override
			public void keyTyped(KeyEvent e) {
			}

			@Override
			public void keyReleased(KeyEvent e) {
			}

			@Override
			public void keyPressed(KeyEvent e) {
				if (e.getKeyCode() == 10) {
					VindictusCraftingCostCalculator.text = VindictusCraftingCostCalculator.entryField.getText();
					VindictusCraftingCostCalculator.outputWindow.setText("");
					try {
						VindictusCraftingCostCalculator.readItemFromWiki(VindictusCraftingCostCalculator.text);
					} catch (ClientProtocolException e1) {
						e1.printStackTrace();
					} catch (IOException e1) {
						e1.printStackTrace();
					}
				}
			}
		});
		VindictusCraftingCostCalculator.panel.setLayout(this.layout);
		VindictusCraftingCostCalculator.panel.add(new Label("Enter the name of the item or set"));
		VindictusCraftingCostCalculator.panel.add(VindictusCraftingCostCalculator.entryField);

		submit.setPreferredSize(new Dimension(100, 25));
		submit.addActionListener(new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent e) {
				VindictusCraftingCostCalculator.text = VindictusCraftingCostCalculator.entryField.getText();
				VindictusCraftingCostCalculator.outputWindow.setText("");
				try {
					VindictusCraftingCostCalculator.readItemFromWiki(VindictusCraftingCostCalculator.text);
				} catch (ClientProtocolException e1) {
					e1.printStackTrace();
				} catch (IOException e1) {
					e1.printStackTrace();
				}
			}
		});

		VindictusCraftingCostCalculator.panel.add(submit);

		ButtonGroup radioButtonGroup = new ButtonGroup();

		ActionListener actionListener = new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent e) {
				if (e.getActionCommand() == VindictusCraftingCostCalculator.playerCrafted) {
					VindictusCraftingCostCalculator.craftedByPlayer = true;
				} else {
					VindictusCraftingCostCalculator.craftedByPlayer = false;
				}
			}
		};
		this.radioButtonPlayerCrafted.setActionCommand(VindictusCraftingCostCalculator.playerCrafted);
		this.radioButtonPlayerCrafted.addActionListener(actionListener);

		this.radioButtonNpcCrafted.setActionCommand(VindictusCraftingCostCalculator.npcCrafted);
		this.radioButtonNpcCrafted.addActionListener(actionListener);
		this.radioButtonNpcCrafted.setSelected(true);
		radioButtonGroup.add(this.radioButtonPlayerCrafted);
		radioButtonGroup.add(this.radioButtonNpcCrafted);

		VindictusCraftingCostCalculator.panel.add(new JLabel("Crafted by"));
		VindictusCraftingCostCalculator.panel.add(this.radioButtonPlayerCrafted);
		VindictusCraftingCostCalculator.panel.add(this.radioButtonNpcCrafted);

		this.add(VindictusCraftingCostCalculator.panel);

		this.setTitle("Vindictus Crafting Cost Calculator");
		this.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		this.setSize(240, 140);
		this.setVisible(true);
		VindictusCraftingCostCalculator.entryField.requestFocusInWindow();
	}

	public static void readItemFromWiki(String itemName)
			throws ClientProtocolException, IOException {
		HttpClient httpclient = new DefaultHttpClient();
		itemName = itemName.replaceAll(",", " ");
		String[] itemNameSplitted = new String[itemName.split(" ").length + 1];
		itemNameSplitted = itemName.split(" ");

		itemName = "";
		try {
			for (int i = 0; i < itemNameSplitted.length; i++) {
				itemNameSplitted[i] = itemNameSplitted[i].substring(0, 1)
						.toUpperCase() + itemNameSplitted[i].substring(1);
				itemName += itemNameSplitted[i] + "_";
			}
		} catch (StringIndexOutOfBoundsException e) {
			VindictusCraftingCostCalculator.outputWindow.setText("You need to enter something first");
		}
		itemName = itemName.substring(0, itemName.length() - 1);
		VindictusCraftingCostCalculator.text = itemName;
		VindictusCraftingCostCalculator.outputWindow.setText("Searching for: " + itemName);
		HashMap<String, Double> materials = new HashMap<String, Double>();
		boolean foundSomething = false;
		try {
			String uriString = "http://www.vindictuswiki.com/w/index.php?title="
					+ itemName + "&action=edit";
			HttpGet httpget = new HttpGet(new URI(uriString));
			HttpResponse response = httpclient.execute(httpget);
			HttpEntity entity = response.getEntity();
			BufferedReader reader = new BufferedReader(new InputStreamReader(
					entity.getContent()));
			String line;
			while ((line = reader.readLine()) != null) {
				if (!VindictusCraftingCostCalculator.craftedByPlayer) {
					if (line.contains("{{Obtain Fee|")) {
						foundSomething = true;
						VindictusCraftingCostCalculator.outputWindow.setText(VindictusCraftingCostCalculator.outputWindow.getText() + "\n" + line);
						line = line.replace(",", "");
						Double goldCost = Double.parseDouble(line.substring(
								line.lastIndexOf("|") + 5, line.length() - 2));
						materials.put("Gold", goldCost);
					}
					if (line.contains("Expertise")) {
						break;
					}
				} else if (VindictusCraftingCostCalculator.craftedByPlayer) {
					if (line.contains("Expertise")) {
						foundSomething = true;
						materials.put("Gold", 0d);
					}
				}
				VindictusCraftingCostCalculator.parseMaterials(materials, line);
			}

		} catch (Exception e) {
			e.printStackTrace();
		}
		if (foundSomething) {

			Double fee = materials.get("Gold");
			materials.remove("Gold");
			Set<?> set = materials.entrySet();

			Iterator<?> i = set.iterator();
			Double cost = fee;

			String[] labels = new String[materials.keySet().size()];
			labels = materials.keySet().toArray(labels);
			VindictusCraftingCostCalculator.labelLength = labels.length;
			int var = 0;
			Iterator<?> it = materials.values().iterator();
			while (it.hasNext()) {
				labels[var] = labels[var] + " (x"
						+ (int) Double.parseDouble(it.next().toString()) + ")";
				var++;
			}

			new JOptionPaneMultiInput(labels, VindictusCraftingCostCalculator.outputWindow);

			VindictusCraftingCostCalculator.display = new JTextArea();

			VindictusCraftingCostCalculator.adjustWindowSize(labels);
			Double[] results = JOptionPaneMultiInput.results;
			int j = 0;
			Double[] costPerItem = new Double[set.size()];
			VindictusCraftingCostCalculator.outputWindow.setText("You need the following: " + "\n\n");
			if (!VindictusCraftingCostCalculator.craftedByPlayer) {
				VindictusCraftingCostCalculator.outputWindow.setText(VindictusCraftingCostCalculator.outputWindow.getText() + "Crafting fee: " + fee
						+ "\n");
			}
			while (i.hasNext()) {
				costPerItem[j] = results[j];
				@SuppressWarnings("rawtypes")
				Map.Entry me = (Map.Entry) i.next();

				costPerItem[j] = (Double) me.getValue() * costPerItem[j];
				cost += costPerItem[j];
				VindictusCraftingCostCalculator.outputWindow.setText(VindictusCraftingCostCalculator.outputWindow.getText() + me.getKey() + " x"
						+ (int) Double.parseDouble(me.getValue().toString())
						+ " : " + costPerItem[j].intValue() + " Gold" + "\n");
				j++;
			}
			VindictusCraftingCostCalculator.text = VindictusCraftingCostCalculator.text.replace("_", " ");
			VindictusCraftingCostCalculator.outputWindow.setText(VindictusCraftingCostCalculator.outputWindow.getText() + "\n" + VindictusCraftingCostCalculator.text + ": "
					+ cost.intValue() + " Gold");
			VindictusCraftingCostCalculator.outputWindow.setAlignmentX(Component.CENTER_ALIGNMENT);
		}

		else {
			VindictusCraftingCostCalculator.outputWindow.setText(VindictusCraftingCostCalculator.outputWindow.getText() + "\n"
					+ "No item matching the search found");
			VindictusCraftingCostCalculator.outputWindow.setForeground(Color.red);
		}

	}

	// Größe passt noch nicht..
	public static void adjustWindowSize(String[] labels) {
		VindictusCraftingCostCalculator.outputWindow.setRows(labels.length+7);
		VindictusCraftingCostCalculator.vinccc.setSize(VindictusCraftingCostCalculator.vinccc.getWidth(), VindictusCraftingCostCalculator.outputWindow.getHeight()+350);
		VindictusCraftingCostCalculator.vinccc.repaint();
	}

	public static void parseMaterials(HashMap<String, Double> materials,
			String line) {
		if (line.contains("{{Materials|")) {
			VindictusCraftingCostCalculator.outputWindow.setText(VindictusCraftingCostCalculator.outputWindow.getText() + "\n" + line);

			String materialName = line.substring(line.indexOf("|") + 1);

			materialName = materialName.substring(0, materialName.indexOf("|"));
			String materialCountAsString = line.substring(13 + materialName
					.length());
			int nextSeperator = materialCountAsString.indexOf("|");
			if (nextSeperator != -1) {
				materialCountAsString = materialCountAsString.substring(0,
						nextSeperator);
			} else {
				materialCountAsString = materialCountAsString.substring(0,
						materialCountAsString.length() - 2);
			}
			Double materialCount = Double.parseDouble(materialCountAsString);
			materials.put(materialName, materialCount);
		}
	}

	@Override
	public void actionPerformed(ActionEvent e) {
		if (e.getSource() == this.radioButtonPlayerCrafted) {
			VindictusCraftingCostCalculator.craftedByPlayer = true;
		} else if (e.getSource() == this.radioButtonNpcCrafted) {
			VindictusCraftingCostCalculator.craftedByPlayer = false;
		}
	}
}
